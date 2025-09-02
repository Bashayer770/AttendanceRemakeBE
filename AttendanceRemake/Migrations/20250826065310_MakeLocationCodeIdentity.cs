using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceRemake.Migrations
{
    /// <inheritdoc />
    public partial class MakeLocationCodeIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    AttCode = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FingerCode = table.Column<int>(type: "int", nullable: false),
                    IODateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    NodeSerialNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Photo = table.Column<byte[]>(type: "image", nullable: true),
                    TrType = table.Column<int>(type: "int", nullable: false),
                    CurTimPlan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.AttCode);
                });

            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    Floor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DescA = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DescB = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Code = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescA = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DescE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    SerialNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DescA = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DescE = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LocCode = table.Column<int>(type: "int", nullable: false),
                    Floor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.SerialNo);
                });

            migrationBuilder.CreateTable(
                name: "TimingPlan",
                columns: table => new
                {
                    Code = table.Column<int>(type: "int", nullable: false),
                    DescA = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DescE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromTime = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ToTime = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    RmdFromTime = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    RmdToTime = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    IsRamadan = table.Column<bool>(type: "bit", nullable: false),
                    CanGoBefore = table.Column<bool>(type: "bit", nullable: false),
                    Activity = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimingPlan", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmpNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FingerCode = table.Column<int>(type: "int", nullable: false),
                    DeptCode = table.Column<int>(type: "int", nullable: false),
                    NameA = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TimingCode = table.Column<int>(type: "int", nullable: false),
                    JobType = table.Column<int>(type: "int", nullable: false),
                    Sex = table.Column<short>(type: "smallint", nullable: false),
                    CheckLate = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    HasAllow = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InLeave = table.Column<bool>(type: "bit", nullable: false),
                    HasPass = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    LocCode = table.Column<int>(type: "int", nullable: true),
                    RegNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee_1", x => x.EmpNo);
                    table.ForeignKey(
                        name: "FK_Employee_TimingPlan",
                        column: x => x.TimingCode,
                        principalTable: "TimingPlan",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "EmpAllow",
                columns: table => new
                {
                    Serial = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpNo = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RealStartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AllowTimeCode = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DedHr = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpAllow", x => x.Serial);
                    table.ForeignKey(
                        name: "FK_EmpAllow_Employee",
                        column: x => x.EmpNo,
                        principalTable: "Employee",
                        principalColumn: "EmpNo");
                });

            migrationBuilder.CreateIndex(
                name: "IODateTimeTrTypeFingerCode_IX",
                table: "Attendance",
                columns: new[] { "IODateTime", "TrType", "FingerCode" });

            migrationBuilder.CreateIndex(
                name: "PF_FCODE_STATUS_TYPE_IOTIME",
                table: "Attendance",
                columns: new[] { "FingerCode", "Status", "TrType", "IODateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpAllow_EmpNo",
                table: "EmpAllow",
                column: "EmpNo");

            migrationBuilder.CreateIndex(
                name: "_dta_index_Employee_6_2110630562__K2",
                table: "Employee",
                column: "FingerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_TimingCode",
                table: "Employee",
                column: "TimingCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "EmpAllow");

            migrationBuilder.DropTable(
                name: "Floors");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "TimingPlan");
        }
    }
}
