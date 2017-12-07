function MatrixExportExcel(tableobject, sheetname, filename)
{
    $(tableobject).table2excel({
        exclude: 'hidden',
        name: sheetname,
        filename: filename
    });
}

$(document).ready(function () {

    $("#tblmatrix").tableHeadFixer({
        "left": 1,
        'head': true,
        'z-index': 9
    });
    
});

