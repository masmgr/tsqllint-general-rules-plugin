using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLintGeneralRulesPlugin;

internal static class TokenStreamHelper
{
    public static bool TryGetTokenRangeBetween(TSqlFragment? left, TSqlFragment? right, out int startIndex, out int endIndex)
    {
        startIndex = -1;
        endIndex = -1;

        var leftEnd = left?.LastTokenIndex ?? -1;
        var rightStart = right?.FirstTokenIndex ?? -1;
        if (leftEnd < 0 || rightStart < 0 || rightStart <= leftEnd)
        {
            return false;
        }

        startIndex = leftEnd + 1;
        endIndex = rightStart;
        return true;
    }

    public static bool HasToken(IList<TSqlParserToken> tokens, int startIndex, int endIndex, TSqlTokenType tokenType)
    {
        for (var i = startIndex; i < endIndex && i < tokens.Count; i++)
        {
            if (tokens[i].TokenType == tokenType)
            {
                return true;
            }
        }

        return false;
    }
}

