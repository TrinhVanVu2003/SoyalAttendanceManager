using System;
using System.Collections.Generic;
using ClosedXML.Excel;

namespace SoyalAttendanceLog
{
    public class ExcelExportService
    {
        public void Export(string filePath, List<AttendanceLog> logs)
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Attendance Report");

                // Title
                ws.Cell(1, 1).Value = "SOYAL Attendance Report";
                ws.Range(1, 1, 1, 5).Merge();
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 16;
                ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Export time
                ws.Cell(2, 1).Value = "Export Time:";
                ws.Cell(2, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                ws.Cell(3, 1).Value = "Total Records:";
                ws.Cell(3, 2).Value = logs.Count;

                // Header
                int headerRow = 5;

                ws.Cell(headerRow, 1).Value = "Employee Name";
                ws.Cell(headerRow, 2).Value = "Fingerprint ID";
                ws.Cell(headerRow, 3).Value = "Date";
                ws.Cell(headerRow, 4).Value = "Time";
                ws.Cell(headerRow, 5).Value = "Event";

                var headerRange = ws.Range(headerRow, 1, headerRow, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Data
                for (int i = 0; i < logs.Count; i++)
                {
                    int row = headerRow + 1 + i;
                    var log = logs[i];

                    ws.Cell(row, 1).Value = string.IsNullOrWhiteSpace(log.UserName) ? "Unknown" : log.UserName;
                    ws.Cell(row, 2).Value = log.UserId;
                    ws.Cell(row, 3).Value = log.EventTime.ToString("yyyy-MM-dd");
                    ws.Cell(row, 4).Value = log.EventTime.ToString("HH:mm:ss");
                    ws.Cell(row, 5).Value = CleanEvent(log.EventType);

                    ws.Range(row, 1, row, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(headerRow);

                workbook.SaveAs(filePath);
            }
        }

        private string CleanEvent(string eventType)
        {
            if (string.IsNullOrWhiteSpace(eventType))
                return "";

            if (eventType.Contains("FingerPrint") || eventType.Contains("Fingerprint"))
                return "Fingerprint";

            if (eventType.Contains("Invalid card"))
                return "Invalid Card";

            if (eventType.Contains("Fingerprint Error"))
                return "Fingerprint Error";

            return eventType;
        }
    }
}