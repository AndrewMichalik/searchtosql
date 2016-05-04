// Package infix contains utility functions for converting Infix to other formats.
package infix

// ToPostFix converts an infix argument string into postfix notation.
func ToPostFix(s string) string {
	r := []rune(s)
	for i, j := 0, len(r)-1; i < len(r)/2; i, j = i+1, j-1 {
		r[i], r[j] = r[j], r[i]
	}
	return string(r)
}
