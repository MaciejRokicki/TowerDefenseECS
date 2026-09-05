using System.Collections.Generic;
using UnityEngine;

namespace TD.Core.InputManager
{
    public class StackInputContext
    {
        private Stack<BaseInputActionMap> stack;

        public BaseInputActionMap LastActionMap => stack.Count > 0 ? stack.Peek() : null;

        public StackInputContext()
        {
            stack = new Stack<BaseInputActionMap>();
        }

        public void Push(BaseInputActionMap inputActionMap)
        {
            stack.Push(inputActionMap);
            Debug.Log(stack.Count);
        }

        public BaseInputActionMap Pop()
        {
            if (stack.TryPop(out var res))
            {
                Debug.Log(stack.Count);
                return res;
            }

            return null;
        }
    }
}
