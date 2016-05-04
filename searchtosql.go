package main

import "fmt"

func main() {
	fmt.Print("Enter infix string: ")
	infixString, err := ReadFromInput()

	if err != nil {
		fmt.Println("Cannot read input: ", err.Error())
		return
	}

	fmt.Println("Postfix string: ", infixToPostfix(infixString))
	return

}