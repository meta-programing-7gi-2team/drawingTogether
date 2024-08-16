using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private Stack<ICommand> commandStack = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        commandStack.Push(command);
    }
    public void UndoCommand()
    {
        if (commandStack.Count > 0)
        {
            ICommand command = commandStack.Pop();
            command.Undo();
        }
    }

    public void ClearCommands()
    {
        commandStack.Clear();
    }
}
