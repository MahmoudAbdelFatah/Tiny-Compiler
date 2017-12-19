using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyScanner
{
    public class ScannerPhase
    {
        Token token;
        List<KeyValuePair<string, Token_Class>> symboles = new List<KeyValuePair<string,Token_Class>>();
        private List<int> lineNumber = new List<int>();

      public  enum TokenType
        {
            Datatype, ReservedKeyWords, Number, Char, Comment, Error, Identifier, String, Main
        }
      public  enum Datatype
        {
            Int, Float, String, Char
        }

       public enum ReservedKeyWordes
        {
            Read, Write, Repeat, Until, If, Elseif, Else, Then, Return, Endl, Main, End
        }

        public List<int> getlineNumberList()
        {
            return lineNumber;
        }

        private void setSymboles()
        {
            symboles.Add(new KeyValuePair<string, Token_Class>("+", Token_Class.Plus));
            symboles.Add(new KeyValuePair<string, Token_Class>("-", Token_Class.Minus));
            symboles.Add(new KeyValuePair<string, Token_Class>("/", Token_Class.Division));
            symboles.Add(new KeyValuePair<string, Token_Class>("*", Token_Class.Times));

            symboles.Add(new KeyValuePair<string, Token_Class>(">", Token_Class.GreaterThan));
            symboles.Add(new KeyValuePair<string, Token_Class>("<", Token_Class.LessThan));
            symboles.Add(new KeyValuePair<string, Token_Class>(">=", Token_Class.GreaterThanOrEqual));
            symboles.Add(new KeyValuePair<string, Token_Class>("<=", Token_Class.LessThanOrEqual));
            symboles.Add(new KeyValuePair<string, Token_Class>("=", Token_Class.Equal));
            symboles.Add(new KeyValuePair<string, Token_Class>(":=", Token_Class.Assign));

            symboles.Add(new KeyValuePair<string, Token_Class>(")", Token_Class.RightParentheses));
            symboles.Add(new KeyValuePair<string, Token_Class>("(", Token_Class.LeftParentheses));
            symboles.Add(new KeyValuePair<string, Token_Class>("<>", Token_Class.NotEqual));

            symboles.Add(new KeyValuePair<string, Token_Class>(";", Token_Class.SemiColon));
            symboles.Add(new KeyValuePair<string, Token_Class>("}", Token_Class.RightBraces));
            symboles.Add(new KeyValuePair<string, Token_Class>("{", Token_Class.LeftBraces));
            symboles.Add(new KeyValuePair<string, Token_Class>(",", Token_Class.Comma));
            symboles.Add(new KeyValuePair<string, Token_Class>("||", Token_Class.Or));
            symboles.Add(new KeyValuePair<string, Token_Class>("&&", Token_Class.And));
            
        }

        private Token_Class getSymbolName(string symbol)
        {
            foreach (var aSymbol in symboles)
            {
                if (aSymbol.Key == symbol)
                    return aSymbol.Value;
            }
            return Token_Class.NULL;
        }

        public void scanning(string[] sourceCode, ref List<Token> tokens ) {
            setSymboles();
            Token_Class tokenType = Token_Class.NULL;
            string lexeme = "";
            bool comment = false, error=false, stringQWithoutQuate=false;
            int cnt = 0;
            foreach (string line in sourceCode)
            {
                cnt++;
                if (line.Length > 0)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        //Handle: String
                        if (line[i] == '\"' && i + 1 < line.Length && !comment)
                        {
                            lexeme = "";
                            lexeme += line[i];
                            i++;
                            while ( i < line.Length && line[i] != '\"')
                            {
                                lexeme += line[i];
                                if(i==line.Length-1 && line[i]!='\"')
                                {
                                    stringQWithoutQuate = true;
                                    break;
                                }
                                i++;
                               
                            }
                            if (!stringQWithoutQuate)
                            {
                                addInScannerList(ref tokens, lexeme, Token_Class.String, cnt);

                                continue;
                            }
                            else
                            {
                                addInScannerList(ref tokens, lexeme, Token_Class.Error, cnt);
                                stringQWithoutQuate = false;
                                lineNumber.Add(cnt);
                                continue;
                            }
                        }
                        //Handle: comment
                        if (comment)
                        {
                            if (i + 1 < line.Length && line[i] == '*' && line[i + 1] == '/')
                            {
                                lexeme += "*/";
                                comment = false;
                                tokenType = Token_Class.Comment;
                                addInScannerList(ref tokens, lexeme, tokenType, cnt);
                                i++;
                            }
                            else
                            {
                                lexeme += line[i];
                            }
                            continue;
                        }

                        else if (i + 1 < line.Length && line[i] == '/' && line[i+1] == '*')
                        {
                            lexeme += "/*";
                            comment = true;
                            continue;
                        }

                        //Handle: Idetifier
                        lexeme = getIdentifier(line, ref i);
                        if (lexeme != "")
                        {
                            string c= lexeme.First().ToString();
                            lexeme=lexeme.First().ToString().ToUpper()+lexeme.Substring(1);
                            //check if this idetifier is reserved keyword
                            if (Enum.IsDefined(typeof(Token_Class), lexeme))
                            {
                              Token_Class tokenType_ = (Token_Class)  Enum.Parse(typeof(Token_Class), lexeme);
                              addInScannerList(ref tokens, c + lexeme.Substring(1), tokenType_, cnt);
                            }
                            else if (Enum.IsDefined(typeof(Token_Class), lexeme))
                            {
                                addInScannerList(ref tokens, c + lexeme.Substring(1), (Token_Class)Enum.Parse(typeof(Token_Class), lexeme), cnt);
                            }
                            else
                                addInScannerList(ref tokens, c+lexeme.Substring(1), Token_Class.Identifier, cnt);
                            i--; //when return from getIdetifier the i is increased by 1
                            continue;
                        }

                        //Handle: character
                        if (i + 2 < line.Length && line[i] == '\'' && line[i + 2] == '\'')
                        {
                            lexeme = "";
                            lexeme = line[i + 1]+"";
                            i += 2;
                            addInScannerList(ref tokens, lexeme, Token_Class.Char, cnt);
                            continue;

                        }

                        //Handle: number
                        error = false;
                        lexeme =getNumber(line,ref i, ref error);
                        if (lexeme != "")
                        {
                            if (error)
                            {
                                lineNumber.Add(cnt);
                                tokenType = Token_Class.Error ;
                            }
                            else
                                tokenType = Token_Class.Number ;
                            addInScannerList(ref tokens, lexeme, tokenType, cnt);
                            //need to sub -1 from i to start from the char after the number
                            i--;
                            continue;
                        }

                        //Handle: symbols
                        if (i < line.Length)
                        {
                            string symbol = "";
                            Token_Class symbolName  ;
                            if (i + 1 < line.Length)
                            {
                                symbol = line[i] + line[i + 1].ToString();
                                symbolName = getSymbolName(symbol);
                                if (symbolName != Token_Class.NULL)
                                {
                                    addInScannerList(ref tokens, symbol, symbolName, cnt);
                                    i++;
                                    continue;
                                }
                            }
                            symbol = line[i].ToString();
                            symbolName = getSymbolName(symbol);
                            if (symbolName != Token_Class.NULL && !stringQWithoutQuate)
                            {
                                addInScannerList(ref tokens, symbol, symbolName, cnt);
                                continue;
                            }
                            
                        }


                        if (line[i] != ' ' && line[i] != '\t')
                        {
                            lineNumber.Add(cnt);
                            lexeme += line[i];
                            while (i + 1 < line.Length && (line[i + 1] != ' '&& line[i] != '\t'))
                            {
                                lexeme += line[++i];
                            }
                            if (!stringQWithoutQuate)
                            addInScannerList(ref tokens, lexeme, Token_Class.Error , cnt);
                        }
                        
                    }
                }
            }
            if (comment)
            {
                lineNumber.Add(cnt);
                addInScannerList(ref tokens, lexeme, Token_Class.Error , cnt);
            }
        }

        private void addInScannerList(ref List<Token>tokens, string lex, Token_Class tokenType, int lineNumber)
        {
            //scannedCode.Add(new KeyValuePair<string, Token_Class>(s, tokenType));
            token = new Token();
            token.lex=lex;
            token.tokentype=tokenType;
            token.lineNumber= lineNumber;
            tokens.Add(token);
            //scannedCode[s]=tokenType;
        }

        private bool isChar(char c) {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        private bool isDigit(int n) {
            return n >= '0' && n <= '9';
        }

        private bool isDot(char c)
        {
            return c=='.';
        }
    
        private string getIdentifier(string s, ref int idx) {
            string ID="";
            //first char must be character
            if(isChar(s[idx])) {
                ID +=s[idx++];
                while(idx < s.Length && (isChar(s[idx]) || isDigit(s[idx]))) {
                    ID += s[idx++];
                }
            }
            return ID;
        }

        private string getNumber(string s, ref int idx, ref bool error) {
            string number="";
            int dot=0;
            if(isDigit(s[idx])) {
                number +=s[idx++];
                while(idx < s.Length && (isDigit(s[idx]) || isDot(s[idx]) )) {
                    if(isDigit(s[idx])) {
                        number +=s[idx++];
                    } 
                    else if(isDot(s[idx])) {
                        number +=s[idx++];
                        dot++;
                    }   
                }
            }
            if (dot > 1)
                error = true;
            return number;
        }

    }
    public enum Token_Class
    {
        Read, Write, Repeat, Until, If, Elseif, Else, Then, Return, Endl, Main, End ,
         Int, Float, String, Char ,
         Number , Comment, Error, Identifier,
         Plus, Minus, Division ,Times ,GreaterThan ,LessThan, GreaterThanOrEqual,
        LessThanOrEqual,Equal , Assign ,RightParentheses ,LeftParentheses,SemiColon,
        RightBraces, LeftBraces, Comma, Or, And, NULL, NotEqual
         ,Program,Main_Fuction ,Fuction_Statement,Fuction_Declaration , 
           Function_Arguments,     Function_Body,Statements , Statement,Parameter
        , Parameters, Parameters_Cont, Fuction_Name, Repeat_Statement, Else_Statement

        ,Function_Call,Function_Arguments_Cont   ,  Function_Part   , ElseIf_Statement,If_Statement,
                EndIf_Statement ,Or_Operation ,And_Operation
        ,Comment_Statement,letter_DigitComp
        ,Factor,TermDash,Term,Mul_Operation,Expression_Dash,Expression,Assignment_Statement,DataType,Declaration_Statement_Case,Declaration_StatementCont,Declaration_Statement,Write_Statement_Cont,Write_Statement, Read_Statement , Return_Statement , Condition_Operator , Condition_Statement  ,Condition,Condition_Term,Condition_Term_Dash
    }
}
