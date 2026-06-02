# SOYAL Attendance Manager

SOYAL Attendance Manager is a small Windows desktop (WinForms) application to collect and manage attendance/event logs from SOYAL biometric/access-control devices. The app can import logs from the SOYAL device web interface, persist them to a local SQLite database, provide a simple dashboard, and export reports to Excel.

This README summarizes project structure, build/run instructions, configuration and development notes.

Key features
- Import attendance/event logs from SOYAL devices using the device web EventLog.cgi endpoint
- Local storage using SQLite (attendance.db)
- Export reports to Excel (.xlsx) using ClosedXML
- Basic employee management (map fingerprint IDs to employee names)
- Automatic background sync (polling), manual import, and raw TCP command testing
- Simple configuration persisted to config.ini and application logging to logs/app.log

Project structure (important files)
- SoyalAttendanceLog/Program.cs — application entry point
- SoyalAttendanceLog/Form1.cs — main WinForms UI and application logic
- SoyalAttendanceLog/DatabaseService.cs — SQLite database access and schema management
- SoyalAttendanceLog/SoyalWebClient.cs — parser for SOYAL EventLog.cgi (HTTP)
- SoyalAttendanceLog/RawTcpService.cs — send/receive raw TCP hex commands
- SoyalAttendanceLog/FakeSoyalDeviceClient.cs — development/test device client
- SoyalAttendanceLog/SoyalDeviceClient.cs — placeholder for a real TCP device client
- SoyalAttendanceLog/ExcelExportService.cs — Excel export using ClosedXML
- SoyalAttendanceLog/AppConfigService.cs — simple config.ini persistence
- packages.config — NuGet package references used by the project

Requirements
- Windows desktop
- .NET Framework 4.8
- Visual Studio (recommended) — the solution targets .NET Framework 4.8
- NuGet packages (restored automatically): ClosedXML, System.Data.SQLite.Core, DocumentFormat.OpenXml, and dependencies

Build and run
1. Clone the repository and open the solution in Visual Studio.
2. Restore NuGet packages (Visual Studio usually restores them automatically).
3. Build the solution (ensure Target Framework is .NET Framework 4.8).
4. Run the application (the main form is Form1).

Configuration
- config.ini (created in the application working directory) stores three lines in this order: IP, Port, DeviceId.
  - Defaults used when file is missing: IP = 192.168.1.143, Port = 1621, DeviceId = 1
- Database file: attendance.db is created next to the executable when the app initializes the schema.
- Logs: logs/app.log (the app writes informational and error entries here).

Usage notes
- Device Connection: set device IP, port and device ID. Use "Test" to verify TCP connectivity (uses a 3s timeout).
- Load Logs: imports logs from the device web EventLog.cgi endpoint and stores them to the local DB.
- Auto Sync: starts a background loop to periodically fetch logs and insert new records.
- Export Excel: exports the currently-displayed grid to an .xlsx file using ClosedXML.
- Employee mapping: enter a Fingerprint/User ID and name to save an employee. Saved names are applied to stored logs.
- Raw TCP: send a hex command and view the hex/ASCII response. Connection and response timeouts are short by design for testing.

Development notes
- The project includes FakeSoyalDeviceClient for testing. Implement SoyalDeviceClient.ReadLogs and TestConnection to enable direct TCP polling for specific device protocols.
- SoyalWebClient.ParseLogs uses a regular expression to parse EventLog.cgi HTML. If your device returns a different HTML layout or encoding, update the parser accordingly.
- Excel export uses ClosedXML. The NuGet package is referenced in packages.config for .NET Framework 4.8.
- Database migrations are not automated; DatabaseService.InitializeDatabase creates the two tables required by the app (AttendanceLogs and Employees).

Troubleshooting
- If logs are not imported, verify config.ini values and that the device is reachable from the machine running the app.
- Check logs/app.log for error details recorded by the application.
- If SQLite-related runtime errors appear, ensure System.Data.SQLite.Core package is restored and that the chosen platform (x86/x64) matches the SQLite native library requirements.

Contributing
- Fork the repository and open a feature branch for your changes.
- Include tests when applicable and update the README with any new configuration or usage details.

License
- No license file is included in this repository. Add a LICENSE file (for example MIT) if you intend to publish or reuse the project under an open-source license.

Acknowledgements
- This README was generated from the existing project source and is intended to document the current codebase and provide a quick start for developers and users.
