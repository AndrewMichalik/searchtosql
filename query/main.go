package main

import (
	"bufio"
	"fmt"
	"os"
	"strings"
	"github.com/AndrewMichalik/searchtosql/infix"
)

func main() {
	fmt.Print("Enter infix text string: ")
	str, err := ReadFromInput()


	if err != nil {
		fmt.Println("Cannot read input: ", err.Error())
		return
	}

	fmt.Println("Postfix string: ")
	fmt.Println(infix.ToPostfix(str))
	return

}

func ReadFromInput() (string, error) {

	reader := bufio.NewReader(os.Stdin)
	s, err := reader.ReadString('\n')

	return strings.TrimSpace(s), err
}