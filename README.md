# What is this project
A gomoku graphic interface, you can use this interface to play with your own game algorithm

# enviroment
This program is developed with .net 9.0, wpf framework. 
It is recommended to build this project using Visual Studio 2022.

# DLL
The gaming algorithm is implemented with DLL, if you want to add your own algorithm, please compile your code into DLL, make sure you implement the following interface(take Negamax as example):
```cs
        [DllImport(dll_path, EntryPoint = "get_negamax_agent")]
        private static extern IntPtr get_negamax_agent(int board_size);

        [DllImport(dll_path, EntryPoint = "add_agent_stone")]
        private static extern void add_agent_stone(IntPtr agent, int color, int y, int x);

        [DllImport(dll_path, EntryPoint = "find_opt_move_with_alpha_beta")]
        private static extern int find_opt_move_with_alpha_beta(IntPtr agent, int color, ref int rec_y, ref int rec_x, int time_limit);
        
        [DllImport(dll_path, EntryPoint = "print_board")]
        private static extern void print_board(IntPtr agent);
```
the detail can be seen in model/Negamax.cs

You need to modify the dll_path in Model/Negamax.cs, to make sure the program can access your DLL file, and modify the Model/game_agent.cs if necessary.



