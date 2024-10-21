var dtable;
$(document).ready(function () {
    loadData()
});
 
function loadData() {
    dtable = $("#mytable").DataTable({
        "ajax": {
            "url": "/Admin/Order/GetData"
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "applicationUser.phoneNumber" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "totalPrice" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                       <a href="/Admin/Order/Details/${data}" class="btn btn-outline-success fw-bold">
                            Details
                       </a>
                    `
                }
            },
        ]
    })
}

//<a asp-action="Update" asp-route-id="@item.Id" class="btn btn-success">
//    <i class="bi bi-pencil-square"></i>
//</a>