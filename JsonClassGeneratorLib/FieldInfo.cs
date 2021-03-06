﻿// Copyright © 2010 Xamasoft

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator
{
    public class FieldInfo
    {

        public FieldInfo(IJsonClassGeneratorConfig generator, string jsonMemberName, JsonType type, bool usePascalCase, bool useJsonAttributes, bool useJsonPropertyName, IList<object> Examples)
        {
            this.generator = generator;
            this.JsonMemberName = jsonMemberName;
            this.MemberName = jsonMemberName;
            bool constainsSpecialChars = isContainsSpecialChars(MemberName);
            ContainsSpecialChars = constainsSpecialChars;
            if (usePascalCase || useJsonAttributes || useJsonPropertyName || constainsSpecialChars) MemberName = JsonClassGenerator.ToTitleCase(MemberName);
            this.Type = type;
            this.Examples = Examples;
        }
        private IJsonClassGeneratorConfig generator;
        public string MemberName { get; private set; }
        public string JsonMemberName { get; private set; }
        public JsonType Type { get; private set; }
        public IList<object> Examples { get; private set; }
        public bool ContainsSpecialChars { get; set; }
        public int MyProperty { get; set; }

        private bool isContainsSpecialChars(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (!char.IsLetterOrDigit(c) && c!= '_')
                {
                    return true;
                }
            }
            return false;
        }

        internal static string ToTitleCase(string str)
        {
            var sb = new StringBuilder(str.Length);
            var flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(flag ? char.ToUpper(c) : c);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            return sb.ToString();
        }

        public string GetGenerationCode(string jobject)
        {
            var field = this;
            if (field.Type.Type == JsonTypeEnum.Array)
            {
                var innermost = field.Type.GetInnermostType();
                return string.Format("({1})JsonClassHelper.ReadArray<{5}>(JsonClassHelper.GetJToken<JArray>({0}, \"{2}\"), JsonClassHelper.{3}, typeof({6}))",
                    jobject,
                    field.Type.GetTypeName(),
                    field.JsonMemberName,
                    innermost.GetReaderName(),
                    -1,
                    innermost.GetTypeName(),
                    field.Type.GetTypeName()
                    );
            }
            else if (field.Type.Type == JsonTypeEnum.Dictionary)
            {

                return string.Format("({1})JsonClassHelper.ReadDictionary<{2}>(JsonClassHelper.GetJToken<JObject>({0}, \"{3}\"))",
                    jobject,
                    field.Type.GetTypeName(),
                    field.Type.InternalType.GetTypeName(),
                    field.JsonMemberName,
                    field.Type.GetTypeName()
                    );
            }
            else
            {
                return string.Format("JsonClassHelper.{1}(JsonClassHelper.GetJToken<{2}>({0}, \"{3}\"))",
                    jobject,
                    field.Type.GetReaderName(),
                    field.Type.GetJTokenType(),
                    field.JsonMemberName);
            }

        }

        public string GetExamplesText()
        {
            return string.Join(", ", Examples.Take(5).Select(x => JsonConvert.SerializeObject(x)).ToArray());
        }

    }
}
