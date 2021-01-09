using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG
{ 
    public static class ConsoleOutputs
    {
        public static void WelcomeMessage()
        {
            Console.WriteLine(@"
                █▄─▀█▀─▄█─▄─▄─█─▄▄▄─█─▄▄▄▄█
                ██─█▄█─████─███─███▀█─██▄─█
                ▀▄▄▄▀▄▄▄▀▀▄▄▄▀▀▄▄▄▄▄▀▄▄▄▄▄▀
            ");

            Console.WriteLine(@"
                ^    ^
               / \  //\
 |\___/|      /   \//  .\
 /O  O  \__  /    //  | \ \
/     /  \/_/    //   |  \  \
@___@'    \/_   //    |   \   \ 
   |       \/_ //     |    \    \ 
   |        \///      |     \     \ 
  _|_ /   )  //       |      \     _\
 '/,_ _ _/  ( ; -.    |    _ _\.-~        .-~~~^-.
 ,-{        _      `-.|.-~-.           .~         `.
  '/\      /                 ~-. _ .-~      .-~^-.  \
     `.   {            }                   /      \  \
   .----~-.\        \-'                 .~         \  `. \^-.
  ///.----..>    c   \             _ -~             `.  ^-`   ^-_
    ///-._ _ _ _ _ _ _}^ - - - - ~                     ~--,   .-~
                                                          /.-'
            ");
        }
    }
}
