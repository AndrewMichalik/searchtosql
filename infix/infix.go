// Package infix contains utility functions for converting Infix to other formats.
package infix

import (
	"strings"
//	"regexp"
	"github.com/hishboy/gocommons/lang"
)


// ToPostFix converts an infix argument string into postfix notation.
func ToPostfix(str string) (tokens []string, filters []string, err string) {
	// Test stack object
	stack := lang.NewStack()
	stack.Push(str)
	str = stack.Pop().(string)

// AJM: Use https://golang.org/pkg/text/scanner/ ?

	// Remove any leading/trailing spaces [AJM todo, compress any embedded spaces]
	str = strings.TrimSpace(str)

	// Check for unbalanced or improperly placed parens or quotes
		

	// Group quoted strings

	// Tag site specific searches

	// Infix string OK, divide into tokens. Exclude special osbSites escaped characters

	// Restore custom [osbSites] escaped characters

	// Extract site filters

	// Sort the site filters alphabetically (to provide consistent results)

	// Expand to separate tokens per array entry
	// Simple whitespace: s := regexp.MustCompile("[^\\s]+").FindAllString(str, -1)


	// Any search terms remaining?

	// If expression has an implied "AND" (Donald Trump, no quotes)

	// Test with reversal
	r := []rune(str)
	for i, j := 0, len(r)-1; i < len(r)/2; i, j = i+1, j-1 {
		r[i], r[j] = r[j], r[i]
	}
	str = string(r)

	return strings.Fields(str), strings.Fields(str), str
}


