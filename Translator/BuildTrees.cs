using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Translator
{
    class BuildTrees
    {

        public void addTree(List<string[]> listStr)
        {
            //для дерева
            TreeClass tree = new TreeClass();
            foreach (string[] s in listStr)
            {
                for (int i = 0; i < s.Length; i++)
                    switch (s[0])
                    {
                        case "id": { tree.Root = tree.AddNode("(" + s[i] + ", " + s[i + 1] + ") ", tree.Root); } break;
                    }

            }
        }
    }
}