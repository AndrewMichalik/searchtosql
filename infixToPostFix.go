// Package infixToPostFix contains utility functions for converting Infix to Postfix notation.
package infixToPostFix

// infixToPostFix returns its argument string in postfix notation.
func infixToPostFix(s string) string {
	r := []rune(s)
	for i, j := 0, len(r)-1; i < len(r)/2; i, j = i+1, j-1 {
		r[i], r[j] = r[j], r[i]
	}
	return string(r)
}
