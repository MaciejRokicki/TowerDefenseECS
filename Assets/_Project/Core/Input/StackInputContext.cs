using System.Collections.Generic;

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
        }

        public BaseInputActionMap Pop()
        {
            if (stack.TryPop(out var res))
            {
                return res;
            }

            return null;
        }
    }
}
