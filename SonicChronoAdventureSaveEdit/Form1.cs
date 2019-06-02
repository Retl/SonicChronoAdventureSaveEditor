using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SonicChronoAdventureSaveEdit
{
    public partial class frmSCASaveEdit : Form
    {
        private int[] arrInts = new int[256];
        List<int> arrData = new List<int>(256);
        List<int> arrDataHeader = new List<int>(8);
        List<byte> arrDataFooter = new List<byte>(8);
        public frmSCASaveEdit()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            grdSaveData.ColumnCount = 2;
            grdSaveData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            grdSaveData.Columns[0].Name = "Index";
            grdSaveData.Columns[1].Name = "Value";
            MapDataToDataGrid(new List<int>(arrInts), grdSaveData);
        }

        private void MapDataToListView(List<int> data, ListView lsv) {
            lsv.Items.Clear();
            for (int i = 0; i < data.Count; i++) {
                lsv.Items.Add(new ListViewItem());
                lsv.Items[i].Text = data[i].ToString();
            }
        }

        private void MapDataToDataGrid(List<int> data, DataGridView grd)
        {
            grd.Rows.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                string rowName;

                switch (i)
                {
                    case 0:
                        rowName = "StoryVal";
                        break;
                    case 1:
                        rowName = "SubStory1";
                        break;
                    case 2:
                        rowName = "SubStory2";
                        break;
                    case 3:
                        rowName = "SubStory3";
                        break;
                    case 12:
                        rowName = "jetLVL";
                        break;
                    case 13:
                        rowName = "sowLVL";
                        break;
                    case 14:
                        rowName = "modLVL";
                        break;
                    case 15:
                        rowName = "burnLVL";
                        break;
                    case 16:
                        rowName = "StageFrame";
                        break;
                    case 80:
                        rowName = "JetEXP";
                        break;
                    case 81:
                        rowName = "SowEXP";
                        break;
                    case 82:
                        rowName = "ModEXP";
                        break;
                    case 83:
                        rowName = "BurnEXP";
                        break;
                    case 90:
                        rowName = "SwoULCK";
                        break;
                    case 91:
                        rowName = "JetULCK";
                        break;
                    case 92:
                        rowName = "ModULCK";
                        break;
                    case 93:
                        rowName = "BurnULCK";
                        break;
                    case 94:
                        rowName = "Emeralds";
                        break;
                    case 153:
                        rowName = "Rings";
                        break;
                    default:
                         rowName = i.ToString();
                        break;
                }
                    

                string[] rowAsTextArray = { rowName, data[i].ToString()};
                grd.Rows.Add(rowAsTextArray);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            opnSaveFile.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveSaveFile.ShowDialog();
        }

        private void opnSaveFile_FileOk(object sender, CancelEventArgs e)
        {
            arrData.Clear();
            arrDataHeader.Clear();
            arrDataFooter.Clear();
            String filePath = opnSaveFile.FileName;

            //Read the contents of the file into a stream

            using (Stream fileStream = opnSaveFile.OpenFile())
            {
                /*using (StreamReader reader = new StreamReader(fileStream))
                {
                    String fileContent = reader.ReadToEnd();
                    Console.WriteLine(fileContent);
                }*/

                using (BinaryReader2 r = new BinaryReader2(fileStream))
                {
                    int readCount = 0;

                    if (r.BaseStream.Length > 3000) {
                        Console.WriteLine("File seems way too large.");
                        lblStatus.Text = "File seems way too large. Aborting. (" + filePath + ")";
                        arrData.Clear();
                        arrData.Add(-9999);
                        MapDataToDataGrid(arrData, grdSaveData);
                        return;
                    }

                    while (r.BaseStream.Position != r.BaseStream.Length)
                    {
                        try
                        {
                            if (readCount < 1) // Header is always at the front.
                            {
                                arrDataHeader.Add(r.ReadInt32());
                            }
                            else if (readCount >= 257) // Must count the header. We're expecting 256 ints only.
                            {
                                arrDataFooter.Add(r.ReadByte());
                            }
                            else if (readCount >= 300) // Very basic wrong-file protection.
                            {
                                arrData.Clear();
                                arrData.Add(-9999);
                                break;
                            }
                            else
                            {
                                arrData.Add(r.ReadInt32());
                            }
                            readCount++;
                        }
                        catch (System.ArgumentException)
                        {
                            Console.WriteLine("Just as a heads up, you had that ARgument Exception error again.");
                            lblStatus.Text = "Just as a heads up, you had that ARgument Exception error again.";
                        }
                        catch (System.IO.EndOfStreamException)
                        {
                            Console.WriteLine("Just as a heads up, you had that End of Steam Exception this time...");
                            lblStatus.Text = "Just as a heads up, you had that End of Steam Exception this time...";
                        }
                    }
                    lblStatus.Text = "Loaded. (Length: " + r.BaseStream.Length + ")";
                    MapDataToDataGrid(arrData, grdSaveData);
                }
            }
        }

        // A Big-Endian approach to reading these values.
        // Taken directly from: https://stackoverflow.com/a/8621184
        class BinaryReader2 : BinaryReader
        {
            public BinaryReader2(System.IO.Stream stream) : base(stream) { }

            public override int ReadInt32()
            {
                var data = base.ReadBytes(4);
                Array.Reverse(data);
                return BitConverter.ToInt32(data, 0);
            }

            public Int16 ReadInt16()
            {
                var data = base.ReadBytes(2);
                Array.Reverse(data);
                return BitConverter.ToInt16(data, 0);
            }

            public Int64 ReadInt64()
            {
                var data = base.ReadBytes(8);
                Array.Reverse(data);
                return BitConverter.ToInt64(data, 0);
            }

            public UInt32 ReadUInt32()
            {
                var data = base.ReadBytes(4);
                Array.Reverse(data);
                return BitConverter.ToUInt32(data, 0);
            }

        }

        private void saveSaveFile_FileOk(object sender, CancelEventArgs e)
        {
            using (Stream fileStream = new FileStream(saveSaveFile.FileName, FileMode.Create, FileAccess.Write))
            {
                /*using (StreamReader reader = new StreamReader(fileStream))
                {
                    String fileContent = reader.ReadToEnd();
                    Console.WriteLine(fileContent);
                }*/

                using (BinaryWriter w = new BinaryWriter(fileStream))
                {
                    foreach (int i32 in arrDataHeader)
                    {
                        byte[] intBytes = BitConverter.GetBytes(i32);
                        Array.Reverse(intBytes);
                        int reversed = BitConverter.ToInt32(intBytes, 0);
                        w.Write(reversed);
                    }

                    foreach (DataGridViewRow sRow in grdSaveData.Rows)
                    {
                        String sVal = sRow.Cells[1].Value.ToString();
                        int iVal = -1;
                        if (!Int32.TryParse(sVal, out iVal))
                        {
                            lblStatus.Text = "Save file corrupted: An invalid value was entered in the data grid at (" + sRow.Cells[0].Value.ToString() + "). Please fix and re-save.";
                            return;
                        }
                        byte[] intBytes = BitConverter.GetBytes(iVal);
                        Array.Reverse(intBytes);
                        int reversed = BitConverter.ToInt32(intBytes, 0);
                        w.Write(reversed);
                    }

                    foreach (byte b in arrDataFooter)
                    {
                        w.Write(b);
                    }

                    lblStatus.Text = "Saved. (Length: " + w.BaseStream.Length + ")";
                }
            }
        }
    }
}
