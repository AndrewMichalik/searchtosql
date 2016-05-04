using System; 
using System.Text; // stringbuilder... 
using System.Text.RegularExpressions; // Regex... 
using System.Collections; // Hashtable, stack, ... 

namespace oseSearcher
{
	public class ParseToPostfix
	{
		private static Hashtable	Precedence;							// Operator precedences 
		// From HTController Spider:
		// http://www.itlab.musc.edu/docs/perl_regexp/
		// Break at . followed by anything other than alphanumeric (www.vinfo.com OK)
		// Break at anything other than dash, dot (isolated), at sign or alphanumeric
		// const string regexString = @"(\.+[^a-zA-Z0-9])|[^\-\.\@a-zA-Z0-9]+";
		// private const string		m_Delimiters = @"[^\-\@a-zA-Z0-9]+";			// Regex character Delimiters
		// private const string		m_Delimiters = @"(^|[^\.\-\@a-zA-Z0-9]|$)+";	// Regex character Delimiters - Include dot
		private const string		m_Characters = @"[\.\-\@a-zA-Z0-9]+";			// Regex character filter
		private const string		m_Delimiters = @"(^|[^\-\@a-zA-Z0-9]|$)+";		// Regex character Delimiters - Exclude dot (dot OK if embedded)
		private	const string		QUOT_TOKEN	= "\"";								// Quoted string delimiter
		private	const char			QUOT_DELIM	= '&';								// Quoted string delimiter
		private char				QUOT_DELIM_ESC	= Convert.ToChar(128 + Convert.ToByte(QUOT_DELIM));
		private	const string		SITE_TOKEN	= "SITE:";								// Quoted string delimiter
		private	const char			SITE_DELIM	= ':';								// Quoted string delimiter
		private char				SITE_DELIM_ESC	= Convert.ToChar(128 + Convert.ToByte(SITE_DELIM));

		#region Constructors
		public ParseToPostfix() 
		{ 
			// configure precedences 
			Precedence=new Hashtable(); 
			Precedence.Add("(",9); 
			Precedence.Add(")",9); 
			Precedence.Add("NOT",7); 
			Precedence.Add("-",7); 
			Precedence.Add("AND",5); 
			Precedence.Add("+",5); 
			Precedence.Add("OR",3); 
			Precedence.Add("|",3); 
		} 
		#endregion

		#region InfixCheck
		public bool InfixCheck(string Infix, out string[] Tokens, out string[] Filters, out string sError) 
		{
			ArrayList Terms = new ArrayList();
			ArrayList Sites = new ArrayList();
			Tokens = new string[0];
			Filters = new string[0];
			sError = null;

			try 
			{ 
				// Remove any leading/trailing spaces, compress any embedded spaces
				Infix = TrimIntraWords(Infix).Trim();

				// Check for unbalanced or improperly placed parens or quotes
				if (!BalanceCheck(Infix, @"\(", @"\)", ref sError)) return (false);
				if (!BalanceCheck(Infix,  "\"",  "\"", ref sError)) return (false);

				// Group quoted strings
				foreach (string quote in match_all("\".+?\"", Infix))
				{
					// Remove any leading/trailing spaces, compress any embedded spaces
					Infix = Infix.Replace(quote, quote.Replace("\"", "").Replace(' ', QUOT_DELIM_ESC));
				}

				// Tag site specific searches
				Infix = Infix.Replace(SITE_TOKEN, SITE_DELIM_ESC.ToString());

				// Infix string OK, divide into tokens. Exclude special osbSites escaped characters
				Terms = match_all(@"\(|\)|AND|OR|NOT|\+|\||\-|" + m_Characters.Replace("[", "[" + QUOT_DELIM_ESC + SITE_DELIM_ESC), Infix);

				// Restore osbSites escaped characters
				for (int ii=0; ii<Terms.Count; ii++) 
				{
					Terms[ii] = ((string)Terms[ii]).Replace(QUOT_DELIM_ESC, QUOT_DELIM);
					Terms[ii] = ((string)Terms[ii]).Replace(SITE_DELIM_ESC, SITE_DELIM);
				}

				// Extract site filters
				ArrayList Sorted = new ArrayList();
				for (int ii=0; ii<Terms.Count; ii++) 
				{
					if (ParseToSQL.IsSite(Terms[ii].ToString()))
					{
						// Get the site name
						string Site = Terms[ii].ToString().ToLower();

						// Remove this site filter from the general search query
						Terms.RemoveAt(ii);

						// Is this site search negated?
						if ((ii > 0) && ParseToSQL.IsNot(Terms[ii-1].ToString())) 
						{
							Site += " " + ParseToSQL.NotMark;
							Terms.RemoveAt(ii-1);
						}

						// Add this site to the list
						Sorted.Add(Site);
					}
				}
				// Sort the site filters alphabetically
				Sorted.Sort();

				// Expand to separate tokens per array entry
				foreach (string site in Sorted) 
				{
					Sites.AddRange(site.Split(' '));
				}
				Filters = (string[]) Sites.ToArray(typeof(string));

				/*
				// Unsorted
				// Extract filters
				for (int ii=0; ii<Terms.Count; ii++) 
				{
					if (ParseToSQL.IsSite(Terms[ii].ToString()))
					{
						// Add this site to the list
						Sites.Add(Terms[ii].ToString());

						// Remove this site filter from the general search query
						Terms.RemoveAt(ii);

						// Is this site search negated?
						if ((ii > 0) && ParseToSQL.IsNot(Terms[ii-1].ToString())) 
						{
							Sites.Add(ParseToSQL.NotMark);
							Terms.RemoveAt(ii-1);
						}
					}
				}
				Filters = (string[]) Sites.ToArray(typeof(string));
				*/

				// Any search terms?
				if (Terms.Count == 0) return (Filters.Length != 0);

				// If expression starts like +Bush or |Bush
				if (ParseToSQL.IsAndOr(Terms[0].ToString())) 
				{ 
					Terms.RemoveAt(0); 
				} 

				// If expression has an implied "AND" (George Bush, no quotes)
				bool bWasOperand = false;
				for (int ii=0; ii<Terms.Count; ii++) 
				{
					// Adjacent operands (or NOT Operator)?
					string Term = Terms[ii].ToString();
					if (ParseToSQL.IsOperand(Term) || ParseToSQL.IsNot(Term))
					{
						if (bWasOperand)
						{
							Terms.Insert(ii, "+");	// Inserted term, ii OK
							bWasOperand = false;
						}
						else bWasOperand = ParseToSQL.IsOperand(Term);
					}
					else if (ParseToSQL.IsAndOr(Term)) bWasOperand = false;	// Reset flag only on AND or OR
				}

				Tokens = (string[]) Terms.ToArray(typeof(string));
				return (true);
			} 
			catch (Exception ex) 
			{ 
				sError = ex.Message;
				return (false); 
			}
		} 

		private static string TrimIntraWords(string input)
		{
			Regex regEx = new Regex(@"[\s]+");
			return (regEx.Replace(input, " "));
		}

		private bool BalanceCheck(string Infix, string Open, string Close, ref string sError) 
		{
			// Check for unbalanced or improperly placed parens/quotes 
			MatchCollection mc1=Regex.Matches(Infix, Open); 
			MatchCollection mc2=Regex.Matches(Infix, Close); 
			
			// Identical open and close? Just look for even count
			if (Open == Close) return ((mc1.Count % 2) == 0);

			// Balanced?
			if (mc1.Count != mc2.Count) 
			{ 
				sError = "Unbalanced '" + Open + Close + "' characters"; 
				return (false);
			} 

			for (int ii=0; ii<mc1.Count; ii++) 
			{ 
				if (mc1[ii].Index>mc2[ii].Index) 
				{ 
					sError = "Improperly placed '" + Open + Close + "' characters"; 
					return (false);
				} 
			}

			// Success
			return (true);
		}
		#endregion

		#region ToPostfix
		// Convert from Infix to postfix 
		public bool ToPostfix(ref string[] Tokens, int TermLimit, out string sError) 
		{ 
			ArrayList Postfix = new ArrayList();	// RPN output array 
			Stack stk = new Stack();				// Stack used to calculate
			int TermCount = 0;						// Initialize number of search terms
			sError = null;

			try 
			{ 
				// Conversion algorithm main routine 
				// Parse next input string token 
				for (int ii=0; ii<Tokens.Length; ii++) 
				{ 
					// Operand? 
					if (ParseToSQL.IsOperand(Tokens[ii])) 
					{
						Postfix.Add(Tokens[ii]); 
						TermCount++;
					}
					else
					{ 
						switch (Tokens[ii]) 
						{
							// Open bracket? 
							case "(": 
								stk.Push(Tokens[ii]); 
								break; 

							// Closed bracket? 
							case ")": 
								while (true) 
								{ 
									if (stk.Peek().ToString()!="(") 
									{ 
										Postfix.Add(stk.Pop());
									} 
									else 
									{ 
										stk.Pop(); 
										break; 
									} 
								} 
								break; 

							// AND, OR, NOT
							default : 
								if ((stk.Count==0) || (stk.Peek().ToString()=="(") || HasPrecedence(Tokens[ii], stk.Peek().ToString())) 
								{ 
									stk.Push(Tokens[ii]); 
								} 
								else 
								{ 
									Postfix.Add(stk.Pop()); 
									ii--; 
								} 
								break; 
						} 
					}
				} 

				// Too many terms?
				if (TermCount > TermLimit)
				{
					sError = "Sorry, the number of search terms exceeded the limit of " + TermLimit.ToString() + ".";
					return (false); 
				}

				// Pop remaining items until stack is empty 
				while (stk.Count != 0) Postfix.Add(stk.Pop());

				// Generate the operator and operands token list
				Tokens = (string[])Postfix.ToArray(typeof(string));

				// Return Success
				return (true); 
			} 
			catch (Exception ex) 
			{ 
				sError = ex.Message;
				return (false); 
			} 
		}

		private void x_AlphaSwap (ref ArrayList Postfix, Stack Operators)
		{
			// Sort the incoming infix data for alphabetical terms, retain precedence
			if ((Postfix.Count < 2) | (Operators.Count == 0)) return;
			if (!ParseToSQL.IsAndOr(Operators.Peek().ToString())) return;

			// Compare alphabetically, case insensitive
			string a = Postfix[Postfix.Count - 2].ToString();
			string b = Postfix[Postfix.Count - 1].ToString();
			if (String.Compare(a, b, true) < 0) return;

			// Swap entries
			Postfix[Postfix.Count - 2] = b;
			Postfix[Postfix.Count - 1] = a;
		}
		#endregion

		#region HasPrecedence
		// Return true if a has an higher precendece than b 
		private bool HasPrecedence(string a, string b) 
		{ 
			// Group NOTs
			if (ParseToSQL.IsNot(a) && ParseToSQL.IsNot(b)) return (true); 

			// AJM Test: Force operands to front (a b c d + + +) for equal operators
			// return ((int)Precedence[a]>(int)Precedence[b]); 
			return ((int)Precedence[a]>=(int)Precedence[b]); 
		} 
		#endregion

		#region match_all
		// formats the Infix string for the converting method 
		/// <summary>
		/// Perform a global regular expression match on the specified subject string.
		/// </summary>
		/// <param name="pattern">The pattern to search in subject.</param>
		/// <param name="subject">The subject string where the pattern will be applied.</param>
		/// <returns>Returns the array of matches found in the subject string.</returns>
		public static ArrayList match_all(string pattern, string subject)
		{
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.Compiled);
			System.Text.RegularExpressions.Match match;
			ArrayList matches = new ArrayList();
			int count = 0;
			for (match = regex.Match(subject); match.Success; match = match.NextMatch())
			{
				for (int ii=0; ii < match.Groups.Count; ii++)
				{
					matches.Add(match.Groups[ii].Value);
				}
				count++;
			}
			
			return (matches);
		}
		#endregion

		#region Properties

		#region Characters
		public static string Characters 
		{
			get {return (m_Characters);}
		}
		#endregion

		#region Delimiters
		public static string Delimiters 
		{
			get {return (m_Delimiters);}
		}
		#endregion

		#region Delimiter_Quote
		public static char Delimiter_Quote 
		{
			get {return (QUOT_DELIM);}
		}
		#endregion

		#region Delimiter_Site
		public static char Delimiter_Site 
		{
			get {return (SITE_DELIM);}
		}
		#endregion

		#endregion

	}

}
