var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll",
            "type": "GET",
            "datatype": "json",
            "error": function (xhr, error, thrown) {
                console.error(xhr.responseText);
                alert('An error occurred while fetching data. Check console for details.');
            }
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "description", "width": "25%" }, // Ensure field names match your data source
            { "data": "price", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/Admin/Product/Upsert?id=${data}"
                            class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                            <a onClick=Delete('/Admin/Product/Delete/${data}')
                            class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                        </div>
                    `;
                },
                "width": "15%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                    toastr.error('An error occurred while deleting the item.');
                }
            });
        }
    });
}
