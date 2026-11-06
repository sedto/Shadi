
from openpyxl import Workbook
from openpyxl.styles import Font, PatternFill, Border, Side, Alignment
from openpyxl.worksheet.datavalidation import DataValidation
from openpyxl.formatting.rule import FormulaRule
from openpyxl.utils import get_column_letter
from openpyxl.worksheet.page import PageMargins
 
wb = Workbook()
ws = wb.active
ws.title = "Journal de travail"
 
# Colors
blue = "4472C4"
grey = "D9D9D9"
light_grey = "EEEEEE"
green = "00C853"
light_green = "92D050"
yellow = "FFD966"
white = "FFFFFF"
black = "333333"
 
thin = Side(style="thin", color="BFBFBF")
border = Border(left=thin, right=thin, top=thin, bottom=thin)
 
# Column widths
widths = [48, 17, 17, 17, 17, 48]
for i, width in enumerate(widths, 1):
    ws.column_dimensions[get_column_letter(i)].width = width
 
# Title
ws.merge_cells("A1:F1")
ws["A1"] = "JOURNAL DE TRAVAIL"
ws["A1"].font = Font(name="Arial", size=22, bold=True, color=blue)
ws["A1"].alignment = Alignment(horizontal="center")
ws.row_dimensions[1].height = 42
 
# Employee information
fields = [
    ("A3", "NOM", "A4", "Nom de l'employé"),
    ("C3", "CONTACT", "C4", "Email / Téléphone"),
    ("E3", "DATE DU JOUR", "E4", "=TODAY()"),
    ("A6", "SERVICE", "A7", "Service"),
    ("C6", "RESPONSABLE", "C7", "Responsable"),
    ("E6", "DURÉE TOTALE PLANIFIÉE", "E7", "=SUM(D11:D30)")
]
 
for label_cell, label, value_cell, value in fields:
    ws[label_cell] = label
    ws[label_cell].font = Font(bold=True, size=10, color=black)
    ws[value_cell] = value
    ws[value_cell].fill = PatternFill("solid", fgColor=grey if value_cell == "E7" else white)
    ws[value_cell].border = border
    ws[value_cell].alignment = Alignment(vertical="center")
 
# Merge information fields
for rng in ["A4:B4", "C4:D4", "E4:F4",
            "A7:B7", "C7:D7", "E7:F7"]:
    ws.merge_cells(rng)
 
ws["E4"].number_format = "dddd d mmmm yyyy"
ws["E7"].number_format = "[h]:mm"
 
# Instructions
ws.merge_cells("A9:F9")
ws["A9"] = "Remplir les cellules non grisées uniquement. Les cellules grisées se remplissent automatiquement."
ws["A9"].font = Font(italic=True, size=10, color=black)
 
# Table headers
headers = [
    "ACTIVITÉ", "HEURE DE DÉBUT", "HEURE DE FIN",
    "DURÉE TOTALE", "STATUT", "NOTES"
]
 
for col, header in enumerate(headers, 1):
    cell = ws.cell(row=10, column=col, value=header)
    cell.font = Font(bold=True, color=white)
    cell.fill = PatternFill("solid", fgColor=grey)
    cell.font = Font(bold=True, color=black)
    cell.alignment = Alignment(horizontal="center")
    cell.border = border
 
# Activity rows
for row in range(11, 31):
    ws.cell(row, 4, f'=IF(OR(B{row}="",C{row}=""),"",MOD(C{row}-B{row},1))')
    ws.cell(row, 4).number_format = "[h]:mm"
 
    for col in range(1, 7):
        cell = ws.cell(row, col)
        cell.border = border
        cell.alignment = Alignment(vertical="center", wrap_text=True)
 
    for col in [4]:
        ws.cell(row, col).fill = PatternFill("solid", fgColor=light_grey)
 
    ws.row_dimensions[row].height = 43
 
# Status dropdown
dv = DataValidation(
    type="list",
    formula1='"Non commencé,En cours,Terminé"',
    allow_blank=True
)
ws.add_data_validation(dv)
dv.add("E11:E30")
 
# Conditional status colors
for status, color in [
    ("Terminé", green),
    ("En cours", light_green),
    ("Non commencé", yellow)
]:
    ws.conditional_formatting.add(
        "E11:E30",
        FormulaRule(
            formula=[f'E11="{status}"'],
            fill=PatternFill("solid", fgColor=color)
        )
    )
 
# Time formatting
for row in range(11, 31):
    ws.cell(row, 2).number_format = "h:mm AM/PM"
    ws.cell(row, 3).number_format = "h:mm AM/PM"
 
# Alignment
for row in range(11, 31):
    for col in [2, 3, 4, 5]:
        ws.cell(row, col).alignment = Alignment(
            horizontal="center", vertical="center"
        )
 
# Freeze headers and configure printing
ws.freeze_panes = "A11"
ws.sheet_view.showGridLines = False
ws.print_title_rows = "1:10"
ws.page_setup.orientation = "landscape"
ws.page_setup.paperSize = ws.PAPERSIZE_A4
ws.page_setup.fitToWidth = 1
ws.page_setup.fitToHeight = 0
ws.sheet_properties.pageSetUpPr.fitToPage = True
ws.page_margins = PageMargins(
    left=0.25, right=0.25, top=0.5, bottom=0.5
)
ws.print_area = "A1:F30"
 
# Save workbook
wb.save("Journal_de_travail.xlsx")
 
print("Excel créé : Journal_de_travail.xlsx")