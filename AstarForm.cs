using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MatrixLib;

namespace AstarLearning
{
    public partial class AstarForm : Form
    {
        public AstarForm()
        {
            InitializeComponent();

            Matrix m = new Matrix(15,10);
            m.Setup(Controls, BlockColor.DefaultColors.Default);
            MatrixColors.GetColorPack(ColorPacksTypes.MainColorPack);

            Run(m);

            var list = new List<Control>();

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].Name.Contains("matrixBlock"))
                {
                    list.Add(Controls[i]);
                }
            }

            foreach (var c in list)
            {
                c.Click += (s, e) =>
                {
                    var set = m.Blocks.Find(p => p.block == c);
                    switch (setType)
                    {
                        case SetTypes.Struct:
                            int type = new Random().Next(0, 4);
                            switch (type)
                            {
                                case 0:
                                    GenerateStructure(m, Structures.GLine, set.coordinates.x, set.coordinates.y);
                                    break;
                                case 1:
                                    GenerateStructure(m, Structures.LLine, set.coordinates.x, set.coordinates.y);
                                    break;
                                case 2:
                                    GenerateStructure(m, Structures.HorLine, set.coordinates.x, set.coordinates.y);
                                    break;
                                case 3:
                                    GenerateStructure(m, Structures.VertLine, set.coordinates.x, set.coordinates.y);
                                    break;
                            }
                            break;
                        case SetTypes.Player:
                            m.SetBlock(Controls, set.coordinates, new List<string>() { "Player" }, BlockColor.CreateBlockColor("_MINT-GREEN"));
                            break;
                        case SetTypes.Finish:
                            m.SetBlock(Controls, set.coordinates, new List<string>() { "Finish" }, BlockColor.CreateBlockColor("_DEEP-CARMINE-PINK"));
                            break;
                        case SetTypes.Void:
                            m.SetBlock(Controls, set.coordinates, new List<string>() { "Empty" }, BlockColor.DefaultColors.Default);
                            break;
                    }
                };
            }

            clearBtn.Click += (s, e) =>
            {
                m.Clear();
            };

            startBtn.Click += async (s, e) =>
            {
                var path = Pathfinder.FindPath(m, m.Blocks.Find(p => p.tags.Contains("Player")).coordinates, m.Blocks.Find(p => p.tags.Contains("Finish")).coordinates);

                foreach (var b in path)
                {
                    if (m.Blocks.Contains(b))
                    {
                        if(!m.GetBlock(b.coordinates).tags.Contains("Finish"))
                            m.SetBlock(Controls, b.coordinates, new List<string>() { "Path" }, BlockColor.CreateBlockColor("_AZURE"));
                        else
                        {
                            m.SetBlock(Controls, b.coordinates, new List<string>() { "Path" }, BlockColor.CreateBlockColor("_DEEP-CARMINE-PINK"));
                        }
                    }
                }


                await Task.Delay(1000);
                for(int i = 0; i < path.Count; i++)
                {
                    var player = m.Blocks.Find(p => p.tags.Contains("Player"));
                    var nextPathList = m.GetNeighbours(player);
                    var nextPath = nextPathList.Find(p => p.tags.Contains("Path"));

                    player.MergeBlock(Controls, nextPath, m);
                    await Task.Delay(5);
                    m.SetBlock(Controls, player.coordinates, new List<string>() { "Empty" }, BlockColor.DefaultColors.Default);

                    await Task.Delay(500);
                }

                MessageBox.Show("Player made it to finish successfully!");
            };
        }

        public async void Run(Matrix m)
        {
            
        }

        public enum Structures
        {
            VertLine, HorLine, LLine, GLine
        }
        public void GenerateStructure(Matrix m, Structures str, int x, int y)
        {
            switch (str)
            {
                case Structures.VertLine:
                    m.SetBlock(Controls, x, y, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK"));

                    try { m.SetBlock(Controls, x, y+1, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    try { m.SetBlock(Controls, x, y-1, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    break;
                case Structures.HorLine:
                    m.SetBlock(Controls, x, y, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK"));

                    try { m.SetBlock(Controls, x+1, y, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    try { m.SetBlock(Controls, x-1, y, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    break;
                case Structures.LLine:
                    m.SetBlock(Controls, x, y, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK"));

                    try { m.SetBlock(Controls, x, y-1, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    try { m.SetBlock(Controls, x, y-2, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    try { m.SetBlock(Controls, x-1, y, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    break;
                case Structures.GLine:
                    m.SetBlock(Controls, x, y, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK"));

                    try { m.SetBlock(Controls, x, y+1, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    try { m.SetBlock(Controls, x, y-1, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    try { m.SetBlock(Controls, x-1, y-1, new List<string>() { "Obstacle" }, BlockColor.CreateBlockColor("_BLACK")); } catch { }
                    break;
            }
        }

        SetTypes setType = SetTypes.Struct;

        enum SetTypes
        {
            Struct, Player, Finish, Void
        }

        private void finishRadio_CheckedChanged(object sender, EventArgs e)
        {
            setType = SetTypes.Finish;
        }
        private void playerRadio_CheckedChanged(object sender, EventArgs e)
        {
            setType = SetTypes.Player;
        }
        private void structRadio_CheckedChanged(object sender, EventArgs e)
        {
            setType = SetTypes.Struct;
        }
        private void voidRadio_CheckedChanged(object sender, EventArgs e)
        {
            setType = SetTypes.Void;
        }
    }
}
