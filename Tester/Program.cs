using TypeScriptDefinitionParser.ContentReaders;
using static TypeScriptDefinitionParser.Parsers.TypeDefinitionParsers;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Types may be of the forms
            //
            //   string
            //   string | number
            //   string[]
            //   string[] | number
            //   { name: string; age: number;  }
            //   { new(): T; }
            //
            // .. (possibly and more that i haven't thought of yet)

            /*
            var content = @"
                declare module __React {
                    // React Elements

                    // ReactElement doc..
                    interface ReactElement<P> {
                        type: string;
                    }
                }";
            */

            var interfaceDetails = Interface(new StringNavigator(
                "interface INamed { name: string; }"
            ));
        }
    }
}
