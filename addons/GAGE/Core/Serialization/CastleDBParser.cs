using SimpleJSON;
using System;
using System.Collections.Generic;

public class CastleDBParser
{
    //TextAsset DBTextAsset;
    private string DBText;

    public RootNode Root { get; private set; }

    public CastleDBParser(string dbText)
    {
        DBText = dbText;
        Root = new RootNode(JSON.Parse(dbText));
    }

    public void RegenerateDB()
    {
        Root = new RootNode(JSON.Parse(DBText));
    }

    public class RootNode
    {
        private JSONNode value;
        public List<SheetNode> Sheets { get; protected set; }

        public RootNode(JSONNode root)
        {
            value = root;
            Sheets = new List<SheetNode>();
            foreach (KeyValuePair<string, SimpleJSON.JSONNode> item in value["sheets"])
            {
                Sheets.Add(new SheetNode(item.Value));
            }
        }

        public SheetNode GetSheetWithName(string name)
        {
            foreach (var item in Sheets)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
    }

    public class SheetNode
    {
        private JSONNode value;
        public bool NestedType { get; protected set; }
        public string Name { get; protected set; }
        public List<ColumnNode> Columns { get; protected set; }
        public List<SimpleJSON.JSONNode> Rows { get; protected set; }

        public SheetNode(JSONNode sheetValue)
        {
            value = sheetValue;
            string rawName = value["name"];
            //for list types the name can come in as foo@bar@boo
            Char delimit = '@';
            var splitString = rawName.Split(delimit);
            if (splitString.Length <= 1)
            {
                Name = value["name"];
                NestedType = false;
            }
            else
            {
                Name = splitString[splitString.Length - 1];
                NestedType = true;
            }
            Columns = new List<ColumnNode>();
            Rows = new List<SimpleJSON.JSONNode>();

            foreach (KeyValuePair<string, SimpleJSON.JSONNode> item in value["columns"])
            {
                Columns.Add(new ColumnNode(item.Value));
            }

            foreach (KeyValuePair<string, SimpleJSON.JSONNode> item in value["lines"])
            {
                Rows.Add(item.Value);
            }
        }
    }

    public class ColumnNode
    {
        private JSONNode value;
        public string TypeStr { get; protected set; }
        public string Name { get; protected set; }
        public string Display { get; protected set; }

        public ColumnNode(JSONNode sheetValue)
        {
            value = sheetValue;
            Name = value["name"];
            Display = value["display"];
            TypeStr = value["typeStr"];
        }
    }
}