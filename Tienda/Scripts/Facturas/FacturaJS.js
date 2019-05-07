var seleccionadas = [];

$(function () {
    var oTable = $('#oTable').DataTable({
        "aoColumnDefs": [
            { "bSortable": false, "aTargets": [0, 1, 3] },
            { "sWidth": "20px", "aTargets": [0, 3] },
            { "sWidth": "80px", "aTargets": [1] }
        ],
        scrollY: "500px",
        scrollX: true,
        scrollCollapse: true,
        "bAutoWidth": false,
        "language": {
            "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
            "search": "Buscar",
            "infoEmpty": "Mostrando 0 to 0 of 0 datos    ",
            "infoFiltered": "(filtrar de _MAX_ datos totales)",
            "lengthMenu": "Mostrar _MENU_ datos",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "zeroRecords": "Ningun registro coincide",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "dom": 'lfBrtip',
        buttons: [
            {
                text: '<span class="icon-eye"></span> Detalles',
                className: 'btnO btnO-sm btn-amaru',
                action: function (e, dt, node, config) {
                    if (seleccionadas.length > 1) {
                        alertify.alert("<h3>Alerta</h3>", "Solo puede haber una fila seleccionada");
                    }
                    else if (seleccionadas.length < 1) {
                        alertify.alert("<h3>Alerta</h3>", "Por favor seleccione una fila");
                    }
                    else {
                        location.href = "/Facturas/Details/" + seleccionadas[0];
                    }
                }
            },
            {
                text: '<span class="icon-pencil"></span> Editar',
                className: 'btnO btnO-sm btn-amaru',
                action: function (e, dt, node, config) {
                    if (seleccionadas.length > 1) {
                        alertify.alert("<h3>Alerta</h3>", "La opción no pude tener multiple selección");
                    }
                    else if (seleccionadas.length < 1) {
                        alertify.alert("<h3>Alerta</h3>", "Por favor seleccione una fila");
                    }
                    else {
                        location.href = "/Facturas/Edit/" + seleccionadas[0];
                    }
                }
            },
            {
                text: '<span class="icon-bin"></span> Borrar',
                className: 'btnO btnO-sm btn-amaru',
                action: function (e, dt, node, config) {
                    if (seleccionadas.length < 1) {
                        alertify.alert("<h3>Alerta</h3>", "Por favor seleccione una fila");
                    }
                    else {
                        alertify.confirm('<b>Confirmar</b>', 'Desea eliminar el registro',
                            function () {
                                location.href = "/Facturas/Delete?list=" + seleccionadas;
                            }
                            ,
                            function () {
                                alertify.error('Cancelado');
                            });
                    }
                }

            },
            //
            { extend: 'excel', text: '<span class="icon-file-excel"></span> Excel', className: 'btnO btnO-sm btn-amaru' },
            { extend: 'print', text: '<span class="icon-printer"></span> Imprimir', className: 'btnO btnO-sm btn-amaru' }
        ]
    });
});

function Seleccionar(id) {
    var find = seleccionadas.find(f => f === id);
    if (find === id) {
        seleccionadas.pop(id);
    }
    else {
        seleccionadas.push(id);
    }
}

$("input[type='checkbox']").change(function (e) {
    if ($(this).is(":checked")) { //If the checkbox is checked
        $(this).closest('tr').addClass("selected");
        //Add class on checkbox checked
    } else {
        $(this).closest('tr').removeClass("selected");
        //Remove class on checkbox uncheck
    }
    Seleccionar($(this).val());
});

$("#ckbAll").click(function () {
    function flagRequestAsSelected(obj) { // I added obj param
        var table = document.getElementById("tbl-appointment-requests");
        for (var i = 0, row; row = table.rows[i]; i++) {
            for (var j = 0, col; col = row.cells[j]; j++) {

                //How to check if checkbox is checked???

                if (obj.checked === true) { // Check if the returned object is checked
                    col.innerHTML = "BLAH";
                }
            }
        }
    }
});