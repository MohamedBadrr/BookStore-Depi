$(document).ready(function () {
    loadUserOrders();
});
function loadUserOrders() {
    userOrdersTable = $("#userOrdersTable").DataTable({
        "ajax": {
            "url": "/Customer/UserOrder/GetUserOrders",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                console.log(json);
                return json.data;
            }
        },
        "columns": [
            { "data": "id" },
            {
                "data": "orderDate",
                "render": function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                }
            },
            { "data": "orderStatus" },
            { "data": "totalPrice" }
        ],
        "language": {
            "emptyTable": "You have not placed any orders yet."
        },
        "width": "100%"
    });
}
