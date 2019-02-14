using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarManager2
{
    enum FARMode
    {
        DIR,
        FILE
    }

    class Layer
    {
        private int selectedItem;
        public int SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (value >= Content.Count)
                {
                    selectedItem = 0;
                }
                else if (value < 0)
                {
                    selectedItem = Content.Count - 1;
                }
                else
                {
                    selectedItem = value;
                }
            }
        }

        public List<FileSystemInfo> Content
        {
            get;
            set;
        }

        public void DeleteSelectedItem()
        {
            FileSystemInfo fileSystemInfo = Content[selectedItem];
            if (fileSystemInfo.GetType() == typeof(DirectoryInfo))
            {
                Directory.Delete(fileSystemInfo.FullName, true);
            }
            else
            {
                File.Delete(fileSystemInfo.FullName);
            }
            Content.RemoveAt(selectedItem);
            selectedItem--;
        }

        public void rename(FileSystemInfo fInfo)

        {

            if (fInfo.GetType() == typeof(DirectoryInfo))

            {
                DirectoryInfo y = fInfo as DirectoryInfo;
                for (int i = 1; i <= 2; i++) // создаем пространство для записи
                {
                    Console.WriteLine();
                }
                for (int i = 0; i < 20; i++)
                {
                    Console.Write(" ");
                }
                Console.Write("Enter new name:"); //введем текст для пользователя 
                string s = Console.ReadLine(); // введем новое название папки
                string path = y.Parent.FullName;
                string newname = Path.Combine(path, s); //путь оригинала объединяем с новым именем папки 
                y.MoveTo(newname); // переместим файл по тому же пути с новым именем 
            }
            else
            {
                FileInfo y = fInfo as FileInfo;
                for (int i = 1; i <= 2; i++) // создание пространства для записи нового имени
                {
                    Console.WriteLine();
                }
                for (int i = 0; i < 20; i++)
                {
                    Console.Write(" ");
                }
                Console.Write("Enter new name:"); //введем текст для пользователя 
                string s = Console.ReadLine();//введем новое название файла
                string newname = Path.Combine(y.Directory.FullName, s);//путь оригинала объединяем с новым именем файла
                y.MoveTo(newname);

            }

        }

        public void Draw()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            for (int i = 0; i < Content.Count; ++i)
            {
                if (i == SelectedItem)//назначенный элемент будет выделяться синим фоном
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }
                else
                    Console.BackgroundColor = ConsoleColor.Black;
                if (Content[i].GetType() == typeof(DirectoryInfo))//если элемент является папкой, то он будет отображаться белым
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;//если элемент является файлом, то он будет отображаться темно-красным цветом
                }
                Console.WriteLine((i + 1) + "." + " " + Content[i].Name);

            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            FARMode mode = FARMode.DIR;
            DirectoryInfo root = new DirectoryInfo(@"C:\Users\user\Desktop\лкд");
            Stack<Layer> history = new Stack<Layer>();
            history.Push(
            new Layer
            {
                Content = root.GetFileSystemInfos().ToList(),
                SelectedItem = 0
            }
            );


            while (true)
            {
                if (mode == FARMode.DIR)
                {
                    history.Peek().Draw();
                }
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();//выбор нескольких клавиш
                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.Delete:
                        history.Peek().DeleteSelectedItem();//удаляем самый последний выбранный объект
                        break;
                    case ConsoleKey.UpArrow:
                        history.Peek().SelectedItem--;//от данного объекта поднимаемся вверх
                        break;
                    case ConsoleKey.DownArrow:
                        history.Peek().SelectedItem++;//от данного объекта спускаемся вниз
                        break;
                    case ConsoleKey.Backspace:
                        if (mode == FARMode.DIR)
                        {
                            history.Pop();
                        }
                        else
                        {
                            mode = FARMode.DIR;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        break;
                    case ConsoleKey.Enter:
                        int gg = history.Peek().SelectedItem;
                        FileSystemInfo fileSystemInfo = history.Peek().Content[gg];
                        if (fileSystemInfo.GetType() == typeof(DirectoryInfo))
                        {
                            DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
                            history.Push(
                            new Layer
                            {
                                Content = directoryInfo.GetFileSystemInfos().ToList(),
                                SelectedItem = 0
                            });
                        }
                        else
                        {
                            mode = FARMode.FILE;
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Black;
                            using (StreamReader sr = new StreamReader(fileSystemInfo.FullName))
                            {
                                Console.WriteLine(sr.ReadToEnd());
                            }
                        }
                        break;
                    case ConsoleKey.F2: // console key for renaming

                        history.Peek().rename(history.Peek().Content[history.Peek().SelectedItem]);
                        break;

                }
            }
        }
    }
}