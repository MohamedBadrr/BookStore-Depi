var dtable;
$(document).ready(function () {
    loadData()
});

function loadData() {
    dtable = $("#mytable").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetData"
        },
        "columns": [
            { "data": "name" },
            { "data": "description" },
            { "data": "price" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                       <a href="/Admin/Product/Update/${data}" class="btn btn-outline-success fw-bold">
                            Update
                       </a>
                    `
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                       <a onClick=DeleteItem("/Admin/Product/Delete/${data}") class="btn btn-outline-danger">
                            Delete
                       </a>
                    `
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                       <a href="/Admin/Product/Details/${data}" class="btn btn-outline-warning">
                            Details
                       </a>
                    `
                }
            }
        ]
    })
}

function DeleteItem(url) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-success",
            cancelButton: "btn btn-danger"
        },
        buttonsStyling: false
    });
    swalWithBootstrapButtons.fire({
        title: "Are you sure?",
        text: "You want to delete this Product ?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dtable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
            swalWithBootstrapButtons.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        } else if (
            result.dismiss === Swal.DismissReason.cancel
        ) {
            swalWithBootstrapButtons.fire({
                title: "Cancelled",
                text: "Your imaginary file is safe :)",
                icon: "error"
            });
        }
    });
}

//<a asp-action="Update" asp-route-id="@item.Id" class="btn btn-success">
//    <i class="bi bi-pencil-square"></i>
//</a>