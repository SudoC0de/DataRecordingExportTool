// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function AddNewColumn() {
    var td = document.createElement('td');

    td.innerHTML = NewColumn();
    document.getElementById("ColumnDefinitions")
            .appendChild(td);
}

function NewColumn() {
    return '<div class="form-group">' +
                '<input class="form-control" />' +
                '<button class="btn btn-primary">Delete</button>' +
           '</div>';
}

function RemoveColumnDefinition(td) {
    document.getElementById("ColumnDefinitions")
            .removeChild(td.parentNode);
}
