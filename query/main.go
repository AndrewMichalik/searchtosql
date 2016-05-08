package main

import (
	"bufio"
	"fmt"
	"os"
	"strings"
	"github.com/AndrewMichalik/searchtosql/infix"
	"github.com/hishboy/gocommons/lang"
)

func main() {
	fmt.Print("Enter infix text string: ")
	infixString, err := ReadFromInput()


	if err != nil {
		fmt.Println("Cannot read input: ", err.Error())
		return
	}

	// Test stack object
	stack := lang.NewStack()
	stack.Push(infixString)
	infixString = stack.Pop().(string)

	fmt.Println("Postfix string: ", infix.ToPostfix(infixString))
	return

}

func ReadFromInput() (string, error) {

	reader := bufio.NewReader(os.Stdin)
	s, err := reader.ReadString('\n')

	return strings.TrimSpace(s), err
}