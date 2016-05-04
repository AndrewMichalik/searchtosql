package main

import "fmt"
import "infix"

func main() {
	fmt.Print("Enter infix text string: ")
	infixString, err := ReadFromInput()

	if err != nil {
		fmt.Println("Cannot read input: ", err.Error())
		return
	}

	fmt.Println("Postfix string: ", infixToPostfix(infixString))
	return

}