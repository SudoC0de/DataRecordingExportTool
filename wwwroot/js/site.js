// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function AddNewColumn() {
    var td = document.createElement('td');
    var columnContainer = document.getElementById("ColumnDefinitions");
    var index = parseInt($("#columnCount").val());

    index++;
    td.innerHTML = NewColumn(index);
    columnContainer.appendChild(td);
    $('#columnCount').val(index);
    $('#' + 'columnsCollection[' + index + '].Name').rules('add', {
        required: true,
        maxlength: 1,
        regex: /^[a-zA-Z0-9]*$/,
        messages: {
            required: "The Name field is required.",
            maxlength: "Column Name cannot be more than 1 character!",
            regex: "Column Name can only accept letters and numbers!"
        }
    });
}

function NewColumn(index) {
    return '<div class="form-group">' +
                '<input class="form-control" ' +
                        'asp-for="columnsCollection[' + index + '].Name" ' +
                        'name="columnsCollection[' + index + '].Name" ' +
                        'type="text" ' +
                        'value="" />' +
                '<span class="text-danger" ' +
                       'asp-validation-for="columnsCollection[' + index + '].Name"></span>' +
                '<input type="button" ' +
                        'class="btn btn-primary" ' +
                        'value="Delete" ' +
                        'onclick="RemoveColumnDefinition(this)"/>' +
           '</div>';
}


function RemoveColumnDefinition(td) {
    document.getElementById("ColumnDefinitions")
            .removeChild(td.parentNode.parentNode);
}
