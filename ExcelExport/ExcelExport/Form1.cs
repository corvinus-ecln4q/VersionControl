﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace ExcelExport
{
    public partial class Form1 : Form
    {
        private int _million = (int)Math.Pow(10, 6);

        RealEstateEntities1 context = new RealEstateEntities1();
        List<Flat> Flats;
        Excel.Application xlApp;
        Excel.Workbook xlWB;
        Excel.Worksheet xlSheet;
        public Form1()
        {
            InitializeComponent();
            CreateExcel();

        }

        private void LoadData()
        {
            Flats = context.Flats.ToList();
        }

        public void CreateExcel()
        {
            try
            {
                xlApp = new Excel.Application();
                xlWB = xlApp.Workbooks.Add(Missing.Value);
                xlSheet = xlWB.ActiveSheet;

                // ...
                CreateTable();

                xlApp.Visible = true;
                xlApp.UserControl = true;
            }
            catch (Exception ex)
            {
                string hiba = string.Format("Error: {0}\nLine: {1}", ex.Message, ex.Source);
                MessageBox.Show(hiba, "Error");

                xlWB.Close(false, Type.Missing, Type.Missing);
                xlApp.Quit();
                xlWB = null;
                xlApp = null;
            }

        }
        private void CreateTable()
        {
            string[] headers = new string[]
            {
                "Kód",
                "Eladó",
                "Oldal",
                "Kerület",
                "Lift",
                "Szobák száma",
                "Alapterület (m2)",
                "Ár (mFt)",
                "Négyzetméter ár (Ft/m2)"
            };

            for (int i = 0; i < headers.Length; i++)
                xlSheet.Cells[1, i + 1] = headers[i];

            object[,] values = new object[Flats.Count, headers.Length];

            int counter = 0;
            int floorColumn = 6;
            foreach (var Flat in Flats)
            {
                values[counter, 0] = Flat.Code;
                values[counter, 1] = Flat.Vendor;
                values[counter, 2] = Flat.Side;
                values[counter, 3] = Flat.District;
                values[counter, 4] = Flat.Elevator
                    ? "Van"
                    : "Nincs";
                values[counter, 5] = Flat.NumberOfRooms;
                values[counter, floorColumn] = Flat.FloorArea;
                values[counter, 7] = Flat.Price;
                values[counter, 8] = string.Format("={0}/{1}*{2}",
                    "H" + (counter + 2).ToString(),
                    GetCell(counter + 2, floorColumn + 1),
                    _million.ToString()); 
                counter++;
            }

            var range = xlSheet.get_Range(
                            GetCell(2, 1),
                            GetCell( 1+ values.GetLength(0), values.GetLength(1)));
            range.Value2 = values;
        }

        private string GetCell(int x, int y)
        {
            string ExcelCoordinate = "";
            int dividend = y;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                ExcelCoordinate = Convert.ToChar(65 + modulo).ToString() + ExcelCoordinate;
                dividend = (int)((dividend - modulo) / 26);
            }

            ExcelCoordinate += x.ToString();
            return ExcelCoordinate;
        }
    }

}
