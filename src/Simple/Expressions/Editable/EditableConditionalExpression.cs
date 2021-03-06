﻿using System;
using System.Linq.Expressions;

namespace Simple.Expressions.Editable
{
    [Serializable]
    public partial class EditableConditionalExpression : EditableExpressionImpl<ConditionalExpression>
    {
        public EditableExpression Test { get; set; }
        public EditableExpression IfTrue { get; set; }
        public EditableExpression IfFalse { get; set; }
        public override ExpressionType NodeType { get; set; }

        public EditableConditionalExpression()
        {
        }

        public EditableConditionalExpression(ConditionalExpression condEx)
        {
            NodeType = condEx.NodeType;
            Test = EditableExpression.Create(condEx.Test);
            IfTrue = EditableExpression.Create(condEx.IfTrue);
            IfFalse = EditableExpression.Create(condEx.IfFalse);
        }

        public EditableConditionalExpression(ExpressionType nodeType, EditableExpression test, EditableExpression ifTrue, EditableExpression ifFalse)
        {
            NodeType = nodeType;
            Test = test;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        // Methods
        public override ConditionalExpression ToTypedExpression()
        {
            return Expression.Condition(Test.ToExpression(), IfTrue.ToExpression(), IfFalse.ToExpression());
        }
    }
}
