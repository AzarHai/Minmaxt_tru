using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinimaxApp
{
    public partial class MainWindow1 : Window
    {
        private Node root;
        public bool isMaxFirst = true;
        public bool analyzeLeftToRight = true;

        public MainWindow1()
        {
            InitializeComponent();
            BuildTree();
        }
        private void OpenWindow2_Click(object sender, RoutedEventArgs e)
        {
            Minmaxt_tru.MainWindow2 window2 = new Minmaxt_tru.MainWindow2();
            window2.Show();
            this.Close();
        }
        public void BuildTree()
        {
            root = new Node(isMaxFirst);

            // Первый уровень
            var child1 = new Node(false);
            var child2 = new Node(false);
            var child3 = new Node(false);

            var child4 = new Node(true);
            var child5 = new Node(true);
            var child6 = new Node(true);
            var child7 = new Node(true);
            var child8 = new Node(true);
            var child9 = new Node(true);
            //var child10 = new Node(true);

            var child11 = new Node(false);
            var child12 = new Node(false);
            var child13 = new Node(false);
            var child14 = new Node(false);
            var child15 = new Node(false);
            var child16 = new Node(false);
            var child17 = new Node(false);
            var child18 = new Node(false);
            var child19 = new Node(false);
            var child20 = new Node(false);
            var child21 = new Node(false);
            var child22 = new Node(false);
            var child23 = new Node(false);
            var child24 = new Node(false);
            var child25 = new Node(false);

            root.Children.Add(child1);
            root.Children.Add(child2);
            root.Children.Add(child3);

            child1.Children.Add(child4);
            child1.Children.Add(child5);

            child2.Children.Add(child6);
            child2.Children.Add(child7);

            child3.Children.Add(child8);
            child3.Children.Add(child9);
            //child3.Children.Add(child10);

            //Ласт
            child4.Children.Add(child11);
            child4.Children.Add(child12);
            child4.Children.Add(child13);

            child5.Children.Add(child14);
            child5.Children.Add(child15);
            child5.Children.Add(child16);

            child6.Children.Add(child17);
            child6.Children.Add(child18);

            child7.Children.Add(child19);
            child7.Children.Add(child20);
            child7.Children.Add(child21);

            child8.Children.Add(child22);
            child8.Children.Add(child23);

            child9.Children.Add(child24);
            child9.Children.Add(child25);

            //цена
            child11.Children.Add(new Node(true, 3));
            child11.Children.Add(new Node(true, 5));

            child12.Children.Add(new Node(true, 6));
            child12.Children.Add(new Node(true, 2));

            child13.Children.Add(new Node(true, 3));
            child13.Children.Add(new Node(true, 4));
            child13.Children.Add(new Node(true, 1));

            child14.Children.Add(new Node(true, 0));
            child14.Children.Add(new Node(true, 2));

            child15.Children.Add(new Node(true, 0));
            child15.Children.Add(new Node(true, 1));
            child15.Children.Add(new Node(true, 1));

            child16.Children.Add(new Node(true, 3));
            child16.Children.Add(new Node(true, 5));
            child16.Children.Add(new Node(true, 4));

            child17.Children.Add(new Node(true, 4));
            child17.Children.Add(new Node(true, 5));

            child18.Children.Add(new Node(true, 6));
            child18.Children.Add(new Node(true, 7));

            child19.Children.Add(new Node(true, 9));
            child19.Children.Add(new Node(true, 8));

            child20.Children.Add(new Node(true, 7));
            child20.Children.Add(new Node(true, 8));
            child20.Children.Add(new Node(true, 7));

            child21.Children.Add(new Node(true, 7));
            child21.Children.Add(new Node(true, 3));

            child22.Children.Add(new Node(true, 2));
            child22.Children.Add(new Node(true, 3));

            child23.Children.Add(new Node(true, 5));
            child23.Children.Add(new Node(true, 3));
            child23.Children.Add(new Node(true, 4));

            child24.Children.Add(new Node(true, 2));
            child24.Children.Add(new Node(true, 1));

            child25.Children.Add(new Node(true, 0));
            child25.Children.Add(new Node(true, 1));
            child25.Children.Add(new Node(true, 0));


            DrawTree();
            PopulateValueEditor();
        }

        public void DrawTree()
        {
            TreeCanvas.Children.Clear();
            DrawNode(root, TreeCanvas.ActualWidth / 2, 20, TreeCanvas.ActualWidth - 60, 0);
        }


        public void DrawNode(Node node, double x, double y, double width, int depth)
        {
            if (node == null) return;

            // Сохраняем координаты узла для последующего выделения
            node.PositionX = x;
            node.PositionY = y;

            // Отрисовка текущего узла
            Ellipse ellipse = new Ellipse
            {
                Width = 35,
                Height = 35,
                Stroke = Brushes.Black,
                Fill = node.IsMax ? Brushes.LightBlue : Brushes.LightGreen
            };
            TreeCanvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, x - 20);
            Canvas.SetTop(ellipse, y - 20);

            TextBlock text = new TextBlock
            {
                Text = node.Value.HasValue ? node.Value.ToString() : "?",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            TreeCanvas.Children.Add(text);
            Canvas.SetLeft(text, x - 10);
            Canvas.SetTop(text, y - 15);

            if (!node.Children.Any()) return;

            double segmentWidth = width / node.Children.Count;

            for (int i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                double childX = x - width / 2 + segmentWidth / 2 + i * segmentWidth;
                double childY = y + 80;

                // Рисуем линию
                Line line = new Line
                {
                    X1 = x,
                    Y1 = y,
                    X2 = childX,
                    Y2 = childY,
                    Stroke = Brushes.Black
                };
                TreeCanvas.Children.Add(line);

                DrawNode(child, childX, childY, segmentWidth, depth + 1);
            }
        }



        public void RebuildTreeButton_Click(object sender, RoutedEventArgs e)
        {
            isMaxFirst = FirstPlayerComboBox.SelectedIndex == 0;
            analyzeLeftToRight = OrderComboBox.SelectedIndex == 0;

            // Обновляем роли всех узлов дерева
            UpdateNodeRoles(root, isMaxFirst);

            // Обнуляем значения всех узлов, которые не являются листами
            ClearNodeValues(root);

            // Перерисовываем дерево с обновленными цветами
            DrawTree();
            PopulateValueEditor();
        }
        public void ClearNodeValues(Node node)
        {
            if (node == null) return;

            // Если это не лист, обнуляем значение
            if (node.Children.Any())
            {
                node.Value = null;
            }

            // Рекурсивно очищаем все дочерние узлы
            foreach (var child in node.Children)
            {
                ClearNodeValues(child);
            }
        }


        public void UpdateNodeRoles(Node node, bool isMaxRole)
        {
            if (node == null) return;

            // Устанавливаем роль текущего узла
            node.IsMax = isMaxRole;

            // Рекурсивно обновляем роли всех дочерних узлов
            foreach (var child in node.Children)
            {
                UpdateNodeRoles(child, !isMaxRole); // Меняем роль на противоположную
            }
        }

        public void RunMinimaxButton_Click(object sender, RoutedEventArgs e)
        {
            Minimax(root, isMaxFirst, analyzeLeftToRight);
            DrawTree();
            HighlightOptimalPath(root);
        }

        public int Minimax(Node node, bool isMaximizingPlayer, bool analyzeLeftToRight)
        {
            if (!node.Children.Any())
                return node.Value.Value;

            var evaluations = node.Children
                .OrderBy(c => analyzeLeftToRight ? node.Children.IndexOf(c) :-node.Children.IndexOf(c))
                .Select(c => Minimax(c, !isMaximizingPlayer, !analyzeLeftToRight));

            node.Value = isMaximizingPlayer ? evaluations.Max() : evaluations.Min();
            return node.Value.Value;
        }

        public void HighlightOptimalPath(Node node)
        {
            Node current = node;
            HighlightNode(current); // Выделяем корень
            while (current.Children.Any())
            {
                current = current.Children
                    .OrderByDescending(c => current.IsMax ? c.Value : -c.Value)
                    .First();
                HighlightNode(current); // Выделяем узел в оптимальном пути
            }
        }

        public void HighlightNode(Node node)
        {
            // Находим фигуры на Canvas, соответствующие узлу
            foreach (var child in TreeCanvas.Children)
            {
                if (child is Ellipse ellipse && Canvas.GetLeft(ellipse) + 20 == node.PositionX &&
                    Canvas.GetTop(ellipse) + 20 == node.PositionY)
                {
                    ellipse.Stroke = Brushes.Red; // Выделяем цветом
                    ellipse.StrokeThickness = 3;  // Делаем границу толще
                }
            }
        }
        public void PopulateValueEditor()
        {
            ValueEditorPanel.Children.Clear();

            int index = 0;
            PopulateValueEditorRecursive(root, ref index);
        }

        public void PopulateValueEditorRecursive(Node node, ref int index)
        {
            if (node == null) return;

            if (node.Children.Count == 0)
            {
                var panel = new StackPanel { Orientation = Orientation.Horizontal };
                panel.Children.Add(new TextBlock { Text = $"Узел {index + 1}:", Width = 80 });

                var textBox = new TextBox
                {
                    Width = 50,
                    Text = node.Value.HasValue ? node.Value.ToString() : "",
                    Tag = node
                };
                textBox.TextChanged += ValueTextBox_TextChanged;

                panel.Children.Add(textBox);
                ValueEditorPanel.Children.Add(panel);
                index++;
            }

            foreach (var child in node.Children)
            {
                PopulateValueEditorRecursive(child, ref index);
            }
        }
        public void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
            {
                if (textBox.Tag is Node node)
                {
                    node.Value = value;
                }
            }
        }


    }

    public class Node
    {
        public List<Node> Children { get; set; }
        public int? Value { get; set; }
        public bool IsMax { get; set; }
        public double PositionX { get; set; } // Координаты на Canvas
        public double PositionY { get; set; } // Координаты на Canvas

        public Node(bool isMax, int? value = null)
        {
            Children = new List<Node>();
            IsMax = isMax;
            Value = value;
        }
    }
}