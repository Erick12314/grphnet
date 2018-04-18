<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio_.aspx.cs" Inherits="BCP.PERCLI.GUI.WEB.Inicio_" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Resumen Cliente 2.0 | Una web de otra galaxia </title>

    <%--Bootstrap--%>

    <link href="bootstrap-3.3.7/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="bootstrap/bootstrap-select.css" rel="stylesheet" />


    <%--Ajax - JQuery--%>

    <script src="bootstrap-3.3.7/js/tests/vendor/jquery.min.js"></script>

    <script src="js/jquery-ui.min2.js" type="text/javascript"></script>

    <link href="js/jquery-ui.css" rel="stylesheet" />

    <script src="bootstrap-3.3.7/js/transition.js"></script>


    <script src="bootstrap-3.3.7/js/modal.js"></script>

    <script src="bootstrap-3.3.7/js/tooltip.js"></script>

    <script src="bootstrap-3.3.7/js/popover.js"></script>
    <%--NUEVA LIBRERIA--%>
    <script src="bootstrap-3.3.7/js/collapse.js"></script>

    <script src="bootstrap-3.3.7/js/tab.js"></script>

    <%--FusionChart Library--%>
    <script type="text/javascript" src="fusioncharts-suite-xt/js/fusioncharts.js"></script>
    <script type="text/javascript" src="fusioncharts-suite-xt/js/themes/fusioncharts.theme.fint.js"></script>

    <%--Scripts de Apoyo--%>
    <script type="text/javascript">
        //Rarorac
        $(document).ready(function () {

            var objeto = $('#realRARORAC');
            var numero = $('#realRARORAC').attr('data-fill');

            setTimeout(function () {
                animate(objeto, numero)
            }, 400);
        });

        //Animación
        function animate(that, percentage) {
            if (!that.hasClass('fill')) return;
            that.removeClass('fill');

            percentage = (100 - percentage) || 0;
            var percentage_initial = 100,
                percentage_current = percentage_initial,
                interval = 0.5;

            var interval_gradient = setInterval(function () {
                that.css(
                  'background',
                  'linear-gradient(#F2F2F2 ' + percentage_current + '%,#FFC000 ' + percentage_current + '%)'
                );
                percentage_current -= interval;
                if (percentage_current <= percentage) clearInterval(interval_gradient);
            }, 5);
            that.addClass('filled');
        };

        //Lista clientes
        $(function () {
            $("#<%=txt_cliente.ClientID%>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: 'Inicio.aspx/ListarClientes_Reporte',
                        data: "{ 'prefix': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return {
                                    label: item.split('//')[1],
                                    val: item.split('//')[0]
                                }
                            }))
                        }
                    });
                },
                search: function (e, u) {
                    $(this).addClass('loader');
                },
                response: function (e, u) {
                    $(this).removeClass('loader');
                }
            });
        });

            //Popup de validación
            function Validacion_Solicitud(titulo) {
                $(function () {
                    $("#dialog_Validacion").dialog({
                        title: titulo,
                        resizable: false,
                        show: {
                            effect: "blind",
                            duration: 1000
                        },
                        hide: {
                            effect: "explode",
                            duration: 1000
                        },
                        width: 400,
                        modal: true,
                        buttons: { "Cerrar": function () { $(this).dialog("close"); } }
                    });
                });
            }

            //up and down RARORAC

            function upFunction() {

                var numero = parseInt(document.getElementById("txtMoverRARORAC").value);

                numero = numero + 1;

                document.getElementById("txtMoverRARORAC").value = numero;

            }

            function downFunction() {

                var numero = parseInt(document.getElementById("txtMoverRARORAC").value);

                numero = numero - 1;

                document.getElementById("txtMoverRARORAC").value = numero;

            }

            function isNumberKey(evt) {
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                    if (charCode != 45 && charCode != 46)
                        return false;
                }
                return true;
            }


            //Primera Barra (MA)
            $(document).ready(function () {

                var objeto = $('#barraMAFill');
                var numero = $('#barraMAFill').attr('data-fill');

                //alert(numero);

                setTimeout(function () {
                    animateBarra(objeto, numero)
                }, 400);
            });

            //Primera Barra (MP)
            $(document).ready(function () {

                var objeto = $('#barraMPFill');
                var numero = $('#barraMPFill').attr('data-fill');

                //alert(numero);

                setTimeout(function () {
                    animateBarra(objeto, numero)
                }, 400);
            });

            //Primera Barra (Comex)
            $(document).ready(function () {

                var objeto = $('#barraComexFill');
                var numero = $('#barraComexFill').attr('data-fill');

                //alert(numero);

                setTimeout(function () {
                    animateBarra(objeto, numero)
                }, 400);
            });

            //Primera Barra (Tesoreria)
            $(document).ready(function () {

                var objeto = $('#barraTesoreriaFill');
                var numero = $('#barraTesoreriaFill').attr('data-fill');

                //alert(numero);

                setTimeout(function () {
                    animateBarra(objeto, numero)
                }, 400);
            });

            //Primera Barra (Otros)
            $(document).ready(function () {

                var objeto = $('#barraOtrosFill');
                var numero = $('#barraOtrosFill').attr('data-fill');

                //alert(numero);

                setTimeout(function () {
                    animateBarra(objeto, numero)
                }, 400);
            });

            //Animación
            function animateBarra(that, percentage) {
                if (!that.hasClass('fillbarra')) return;
                that.removeClass('fillbarra');

                percentage = (100 - percentage) || 0;
                var percentage_initial = 100,
                    percentage_current = percentage_initial,
                    interval = 0.5;

                var interval_gradient = setInterval(function () {
                    that.css(
                      'background',
                      'linear-gradient(to left, rgb(255,255,2555) ' + percentage_current + '%,rgb(230,0,0) ' + percentage_current + '%)'
                    );
                    percentage_current -= interval;
                    if (percentage_current <= percentage) clearInterval(interval_gradient);
                }, 5);
                that.addClass('filled');
            };


    </script>

    <style>
        @font-face
        {
            font-family: "Flexo-bold";
            src: url("fonts/Durotype - Flexo-Bold_0.otf");
        }

        @font-face
        {
            font-family: "Flexo-normal";
            src: url("fonts/Durotype - Flexo-Medium_0.otf");
        }

        #gvData1
        {
            margin: 30px auto 1px auto;
        }
    </style>

    <%--CSS--%>

    <link rel="stylesheet" type="text/css" href="css/RCLIestilos.css" />

</head>

<body>
    <form id="form1" runat="server">


        <%--Menú Resumen Cliente--%>
        <nav class="navbar navbar-default" role="navigation">
            <div class="container-fluid">
                <div class="navbar-header">
                    <%--Responsive--%>
                    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-ex1-collapse">
                        <span class="sr-only">Desplegar navegación</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <a class="navbar-brand" href="#">
                        <span>
                            <asp:Image ID="img_logo" runat="server" ImageUrl="img/icon_logo.png" Width="35px" Style="margin-top: -9px" />
                            <img src="img/icon_name.PNG" style="margin-top: -9px" />
                        </span>
                    </a>
                </div>

                <div class="collapse navbar-collapse navbar-ex1-collapse">
                    <ul class="nav navbar-nav">
                        <%--<li class="active"><a href="#">Inicio</a></li>--%>
                    </ul>
                    <div class="nav navbar-nav navbar-right">
                        <li>
                            <asp:Panel ID="pnlControlEnter" runat="server" DefaultButton="btn_search">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txt_cliente" CssClass="form-control" runat="server" placeholder="Buscar Cliente, IDC..." Style="width: 480px; margin-top: 8px; z-index: 1; position: relative" ClientIDMode="Static"></asp:TextBox>
                                        </td>
                                        <td>
                                            <div class="navbar-form">
                                                <asp:LinkButton ID="btn_search" runat="server" CssClass="btn btn-default" OnClick="btn_buscar_indicador" Style="border-style: hidden; background: rgba(0,0,0, 0);">
                                                        <img src="img/icon_search.png" width="25px"/>
                                                </asp:LinkButton>
                                            </div>
                                        </td>
                                        <td>

                                            <%--<div class="navbar-form">
                                                <asp:LinkButton ID="btn_print" runat="server" CssClass="btn btn-default" Style="border-style: hidden; background: rgba(0,0,0, 0);">
                                                       <img src="img/icon_print.png" width="25px" /> 
                                                </asp:LinkButton>
                                            </div>--%>

                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </li>
                    </div>
                </div>
            </div>
        </nav>

        <%--Informacion del Cliente--%>
        <div id="container">

            <div id="boxheader">
                <asp:Label ID="lblClient" runat="server" CssClass="lblClient" Text=""></asp:Label>
                <asp:Label ID="lblClientType" runat="server" CssClass="lblheader" Text=""></asp:Label>
                <asp:Label ID="lblRating" runat="server" CssClass="lblheader" Text=""></asp:Label>
                <asp:Label ID="lblRisk" runat="server" CssClass="lblheader" Text=""></asp:Label>
                <asp:Label ID="lblMaxCEM" runat="server" CssClass="lblheader" Text=""></asp:Label>
                <asp:Label ID="lblUtilCEM" runat="server" CssClass="lblheader" Text=""></asp:Label>
            </div>

            <div id="d1" class="box notlast">
                <div class="topborder c1"></div>

                <div class="box-content">
                    <div class="box-header">
                        RARORAC
                    </div>
                    <div id="boxPpto">
                        <div class="icon-box">
                            <img src="img/icon-ppto.png" />
                        </div>
                        <label id="ppto_RARORACGA" runat="server"></label>
                    </div>

                    <div runat="server" id="boxbodyRARORAC" style="display: none;" class="box-body">

                        <asp:Label ID="cumpRarorac" CssClass="cumpRARORACLabel" runat="server" Text="Cump:"></asp:Label>

                        <div id="realRARORAC" runat="server" class="circle fill" data-fill="42">
                            <asp:Label ID="lblDescRARORAC" runat="server" Text="Real"></asp:Label>
                            <asp:Label ID="lblRARORAC" runat="server" CssClass="circle-text" Text=""></asp:Label>

                            <asp:Label ID="lblDescCliente" runat="server" Text="Cliente"></asp:Label>
                        </div>


                        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

                        <%--Popup-Validación --%>
                        <div id="dialog_Validacion" title="" style="display: none">
                            <p>
                                &nbsp;
                                    <asp:UpdatePanel ID="UpdatePanelValidacion" runat="server">
                                        <ContentTemplate>
                                            <div>
                                                <%=validacion %>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                            </p>
                        </div>

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div id="boxSimuladorRARORAC">
                                    <asp:Button ID="btnSimuladorUp" CssClass="btnSimulador btnUp" OnClientClick="upFunction()" runat="server" Text="" />
                                    <%--<div id="btnSimuladorUp" onclick="upFunction()" class="btnSimulador btnUp"></div>--%>
                                    <asp:TextBox ID="txtMoverRARORAC" onkeypress="return isNumberKey(event)" runat="server" Text="0%" CssClass="cajaSimular" OnTextChanged="txtMoverRARORAC_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:Button ID="btnSimuladorDown" CssClass="btnSimulador btnDown" OnClientClick="downFunction()" runat="server" Text="" />

                                    <div id="flechitaRARORAC">
                                        <div id="rectangleRARORAC"></div>
                                        <div id="triangleRARORAC"></div>
                                    </div>

                                </div>

                                <div id="RARORACCartera">
                                    <asp:Label ID="realCARTERA" CssClass="realRARORACCartera" runat="server">21%</asp:Label>
                                    <asp:Label ID="lblCartera" runat="server">Cartera</asp:Label>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </div>

                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

            <div id="d2" class="box notlast">
                <div class="topborder c1"></div>
                <div class="box-content">
                    <div class="box-header">
                        Trend de RARORAC (M S/)
                        <!--Aquí va el titulo-->
                    </div>
                    <div class="box-body">
                        <!--Aquí va el contenido-->

                        <div id="divFormulaRARORAC">
                            RARORAC = (UNE+PROV-PE)/KE
                        </div>

                        <asp:GridView ID="dgvTrendRARORAC" AutoGenerateColumns="False" runat="server" CssClass="dgvClass" OnRowDataBound="gv_RowDataBoundRARORAC">
                            <Columns>
                                <asp:BoundField DataField="INDICADOR" HeaderText="">
                                    <HeaderStyle CssClass="sCabecera_principal" />
                                    <ItemStyle CssClass="sCabecera_principal" />
                                </asp:BoundField>

                                <asp:BoundField DataField="201702" HeaderText="FEB">
                                    <HeaderStyle CssClass="sCabecera_principal" />
                                    <ItemStyle CssClass="sItemTRARORAC" />
                                </asp:BoundField>

                                <asp:BoundField DataField="201703" HeaderText="MAR">
                                    <HeaderStyle CssClass="sCabecera_principal" />
                                    <ItemStyle CssClass="sItemTRARORAC" />
                                </asp:BoundField>

                                <asp:BoundField DataField="201704" HeaderText="ABR">
                                    <HeaderStyle CssClass="sCabecera_principal" />
                                    <ItemStyle CssClass="sItemTRARORAC" />

                                </asp:BoundField>

                                <asp:BoundField DataField="201705" HeaderText="MAY">
                                    <HeaderStyle CssClass="sCabecera_principal" />
                                    <ItemStyle CssClass="sItemTRARORAC" />
                                </asp:BoundField>

                                <asp:BoundField DataField="201706" HeaderText="JUN">
                                    <HeaderStyle CssClass="sCabecera_principal" />
                                    <ItemStyle CssClass="sItemTRARORAC" />
                                </asp:BoundField>

                                <asp:BoundField DataField="201707" HeaderText="JUL">
                                    <HeaderStyle CssClass="sCabecera_principal" />
                                    <ItemStyle CssClass="sItemTRARORAC" />
                                </asp:BoundField>

                            </Columns>

                        </asp:GridView>

                    </div>
                </div>

                <a class="icon-box icon-help" href="#">
                    <img src="img/icon-help.png" /></a>
                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

            <div id="d3" class="box">
                <div class="topborder c2"></div>
                <div class="box-content">
                    <div class="box-header">
                        <table style="width: 100%">
                            <tr>
                                <td>Indicadores del file</td>
                                <td style="text-align: right">
                                    <asp:HyperLink ID="hlFDG" runat="server" Target="_blank" NavigateUrl="http://10.80.218.93:9007/Views/Reporte/IndicadoresFile.aspx" ForeColor="#0070CA">Ir a FDG para más detalle</asp:HyperLink></td>
                            </tr>
                        </table>
                        <!--Aquí va el titulo-->
                    </div>
                    <div class="box-body">
                        <!--Aquí va el contenido-->

                        <div id="dGridView">
                            <asp:GridView ID="gvData1" runat="server" AutoGenerateColumns="False" OnRowCreated="gvData1_RowCreated">
                                <Columns>

                                    <asp:BoundField DataField="INDICADOR" HeaderText="">
                                        <HeaderStyle CssClass="sCabecera_secundaria sCol0" />
                                        <ItemStyle CssClass="sItem sCol0" />
                                    </asp:BoundField>

                                    <asp:TemplateField ItemStyle-CssClass="sItem sCol15912" HeaderText="% Relev.">
                                        <HeaderStyle CssClass="sCabecera_secundaria sCol15912" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblRelevTT" runat="server" Text='<%# Eval("COL1") %>' ToolTip="Relevancia acum. año actual"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="COL2" HeaderText="MAY">
                                        <HeaderStyle CssClass="sCabecera_secundaria" />
                                        <ItemStyle CssClass="sItem" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="COL3" HeaderText="JUN">
                                        <HeaderStyle CssClass="sCabecera_secundaria" />
                                        <ItemStyle CssClass="sItem" />
                                    </asp:BoundField>

                                    <asp:TemplateField ItemStyle-CssClass="sItem" HeaderText="VAR(%)">
                                        <HeaderStyle CssClass="sCabecera_secundaria" />
                                        <ItemTemplate>
                                            <table>
                                                <tr>
                                                    <td style="width: 40px; text-align: center">
                                                        <asp:Label ID="lblCOL4" runat="server" Text='<%# Eval("COL4") %>' CssClass="sItem"></asp:Label>
                                                    </td>
                                                    <td style="width: 10px;">
                                                        <asp:Image ID="img1" runat="server" ImageUrl='<%# Eval("COL4_IMG") %>' Width="10px" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="COL5" HeaderText="ACUM17">
                                        <HeaderStyle CssClass="sCabecera_secundaria sCol15912" />
                                        <ItemStyle CssClass="sItem sCol15912" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="COL6" HeaderText="JUN16">
                                        <HeaderStyle CssClass="sCabecera_secundaria sCol678" />
                                        <ItemStyle CssClass="sItem sCol678" />
                                    </asp:BoundField>
                                    <asp:TemplateField ItemStyle-CssClass="sItem sCol678" HeaderText="VAR(%)">
                                        <HeaderStyle CssClass="sCabecera_secundaria sCol678" />
                                        <ItemTemplate>
                                            <table>
                                                <tr>
                                                    <td style="width: 40px; text-align: center">
                                                        <asp:Label ID="lblCOL7" runat="server" Text='<%# Eval("COL7") %>' CssClass="sItem"></asp:Label>
                                                    </td>
                                                    <td style="width: 10px;">
                                                        <asp:Image ID="imgCOL7" runat="server" ImageUrl='<%# Eval("COL7_IMG") %>' Width="10px" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="COL8" HeaderText="ACUM16">
                                        <HeaderStyle CssClass="sCabecera_secundaria sCol678" />
                                        <ItemStyle CssClass="sItem sCol678" />
                                    </asp:BoundField>

                                    <asp:TemplateField ItemStyle-CssClass="sItem sCol15912" HeaderText="VAR(%)">
                                        <HeaderStyle CssClass="sCabecera_secundaria sCol15912" />
                                        <ItemTemplate>
                                            <table>
                                                <tr>
                                                    <td style="width: 40px; text-align: center">
                                                        <asp:Label ID="lblCOL9" runat="server" Text='<%# Eval("COL9") %>' CssClass="sItem"></asp:Label>
                                                    </td>
                                                    <td style="width: 10px;">
                                                        <asp:Image ID="imgCOL9" runat="server" ImageUrl='<%# Eval("COL9_IMG") %>' Width="10px" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="COL10" HeaderText="MAY">
                                        <HeaderStyle CssClass="sCabecera_secundaria" />
                                        <ItemStyle CssClass="sItem" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="COL11" HeaderText="JUN">
                                        <HeaderStyle CssClass="sCabecera_secundaria" />
                                        <ItemStyle CssClass="sItem" />
                                    </asp:BoundField>

                                    <asp:TemplateField ItemStyle-CssClass="sItem sCol15912" HeaderText="VAR(%)">
                                        <HeaderStyle CssClass="sCabecera_secundaria sCol15912" />
                                        <ItemTemplate>
                                            <table>
                                                <tr>
                                                    <td style="width: 40px; text-align: center">
                                                        <asp:Label ID="lblCOL12" runat="server" Text='<%# Eval("COL12") %>' CssClass="sItem"></asp:Label>
                                                    </td>
                                                    <td style="width: 10px;">
                                                        <asp:Image ID="img4" runat="server" ImageUrl='<%# Eval("COL12_IMG") %>' Width="10px" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>

                    </div>
                </div>
                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

            <div style="clear: both; margin-bottom: 10px;"></div>

            <%--Colocaciones--%>
            <div id="d4" class="box notlast">

                <div class="topborder c2"></div>

                <div class="box-content">
                    <div class="box-header">
                        Colocaciones directas (MM S/)
                        <!--Aquí va el titulo-->
                    </div>

                    <div class="box-body">
                        <!--Aquí va el contenido-->
                        <asp:Literal ID="Panel_CD" runat="server"></asp:Literal>
                    </div>
                </div>

                <a id="zoom_CD" class="icon-box" data-toggle="modal" data-target="#myModalCD">
                    <img src="img/icon-zoom.png" />
                </a>

                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

            <%--Depositos--%>
            <div id="d5" class="box notlast">
                <div class="topborder c2"></div>
                <div class="box-content">
                    <div class="box-header">
                        Depósitos (MM S/)
                        <!--Aquí va el titulo-->
                    </div>
                    <div class="box-body">
                        <!--Aquí va el contenido-->
                        <asp:Literal ID="Panel_DP" runat="server"></asp:Literal>
                    </div>
                </div>

                <a id="zoom_DP" class="icon-box" data-toggle="modal" data-target="#myModalDP">
                    <img src="img/icon-zoom.png" />
                </a>

                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

            <%--Contingente--%>
            <div id="d6" class="box notlast">
                <div class="topborder c2"></div>
                <div class="box-content">
                    <div class="box-header">
                        Contingentes (MM S/)
                        <!--Aquí va el titulo-->
                    </div>
                    <div class="box-body">
                        <!--Aquí va el contenido-->
                        <asp:Literal ID="Panel_CT" runat="server"></asp:Literal>
                    </div>
                </div>

                <a id="zoom_CT" class="icon-box" data-toggle="modal" data-target="#myModalCT">
                    <img src="img/icon-zoom.png" />
                </a>


                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

            <div id="d7" class="box">
                <div class="topborder c2"></div>
                <div class="box-content">
                    <div class="box-header">
                        Ingresos totales (M S/)
                        <!--Aquí va el titulo-->
                    </div>
                    <div class="box-body">
                        <!--Aquí va el contenido-->
                        <asp:Literal ID="Panel_IT" runat="server"></asp:Literal>
                    </div>
                </div>

                <a id="zoom_IT" class="icon-box" data-toggle="modal" data-target="#myModalIT">
                    <img src="img/icon-zoom.png" />
                </a>

                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

            <div style="clear: both; margin-bottom: 10px;"></div>

            <div id="d8" class="box notlast">
                <div class="topborder c2"></div>
                <div class="box-content">
                    <div class="box-header">
                        Distribución de ingresos (M S/)
                        <!--Aquí va el titulo-->
                    </div>
                    <div class="box-body">
                        <!--Aquí va el contenido-->
                        <asp:Literal ID="Panel_IXS" runat="server"></asp:Literal>
                    </div>
                </div>

                <a id="A1" class="icon-box" data-toggle="modal" data-target="#myModalIXS">
                    <img src="img/icon-zoom.png" />
                </a>

                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>
            <%--Uso de Productos--%>
            <div id="d9" class="box notlast">
                <div class="topborder c3"></div>
                <div class="box-content">
                    <div class="box-header">
                        Uso de productos
                    </div>
                    <div class="box-body">
                        <div id="dUsoProducto" runat="server" style="display: none; margin: 10px auto 10px auto; width: 100%; height: 100%;">
                            <div class="progress_bar">
                                <div class="progress_bar_title">
                                    <span>M. Activo</span>
                                </div>

                                <div id="barraMA" runat="server">
                                    <div id="barraMAFill" runat="server" data-fill="80" class="progress_bar_all fillbarra">
                                        <div id="barraMAText" runat="server" class="progress_bar_text"></div>
                                    </div>
                                </div>
                            </div>

                            <div class="progress_bar">
                                <div class="progress_bar_title">
                                    <span>M. Pasivo</span>
                                </div>
                                <div id="barraMP" runat="server">
                                    <div id="barraMPFill" runat="server" data-fill="80" class="progress_bar_all fillbarra">
                                        <div id="barraMPText" runat="server" class="progress_bar_text"></div>
                                    </div>
                                </div>
                            </div>

                            <div class="progress_bar">
                                <div class="progress_bar_title">
                                    <span>Comex</span>
                                </div>
                                <div id="barraComex" runat="server">
                                    <div id="barraComexFill" runat="server" data-fill="80" class="progress_bar_all fillbarra">
                                        <div id="barraComexText" runat="server" class="progress_bar_text"></div>
                                    </div>
                                </div>
                            </div>

                            <div class="progress_bar">
                                <div class="progress_bar_title">
                                    <span>Tesorería</span>
                                </div>
                                <div id="barraTesoreria" runat="server">
                                    <div id="barraTesoreriaFill" runat="server" data-fill="80" class="progress_bar_all fillbarra">
                                        <div id="barraTesoreriaText" runat="server" class="progress_bar_text"></div>
                                    </div>
                                </div>
                            </div>

                            <div class="progress_bar">
                                <div class="progress_bar_title">
                                    <span>Otros</span>
                                </div>
                                <div id="barraOtros" runat="server">
                                    <div id="barraOtrosFill" runat="server" data-fill="80" class="progress_bar_all fillbarra">
                                        <div id="barraOtrosText" runat="server" class="progress_bar_text"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <a id="zoom_UsoProducto" class="icon-box" data-toggle="modal" data-target="#myModalUsoProducto">
                    <img src="img/icon-zoom.png" />
                </a>
                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

            <%--Distribucion de PDM--%>
            <div id="d10" class="box notlast">
                <div class="topborder c4"></div>
                <div class="box-content">
                    <div class="box-header">
                        Distribución de PDM
                        <!--Aquí va el titulo-->
                        <asp:Literal ID="Panel_DPDM" runat="server"></asp:Literal>
                    </div>
                    <div class="box-body">
                        <!--Aquí va el contenido-->
                    </div>
                </div>

                <a id="zoom_DistPDM" class="icon-box" data-toggle="modal" data-target="#myModalDistPDM">
                    <img src="img/icon-zoom.png" />
                </a>

                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

            <%--Trend de PDM--%>
            <div id="d11" class="box notlast">
                <div class="topborder c4"></div>
                <div class="box-content">
                    <div class="box-header">
                        Trend de PDM
                        <!--Aquí va el titulo-->
                    </div>
                    <div class="box-body">
                        <!--Aquí va el contenido-->
                        <asp:Literal ID="Panel_TPDM" runat="server"></asp:Literal>
                    </div>
                </div>

                <%--cambian el id--%>


                <a id="zoom_TrendPDM" class="icon-box" data-toggle="modal" data-target="#myModalTrendPDM">
                    <img src="img/icon-zoom.png" />
                </a>

                <div class="box-corner1"></div>
                <div class="box-corner2"></div>
            </div>

        </div>


        <%--Colocaciones--%>
        <div id="myModalCD" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">

            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="topborder c2"></div>

                    <div class="modal-header">
                        <div class="box-header">
                            <h4>Colocaciones Directas (MM S/)</h4>
                            <img src="img/exit-modal.png" class="btn-exit-modal" data-dismiss="modal" />
                        </div>
                    </div>

                    <div class="modal-body">
                        <ul id="Ul1" class="ingresos nav nav-tabs">
                            <li class="active"><a href="#homeCD" data-toggle="tab">Corto Plazo</a></li>
                            <li><a href="#profileCD" data-toggle="tab">Largo Plazo</a></li>
                        </ul>

                        <div id="myTabContentCD" class="tab-content">
                            <div class="tab-pane fade in active" id="homeCD">
                                <div style="text-align: center; height: 400px; width: 840px">
                                    <asp:Literal ID="Panel_CD_Zoom_CP" runat="server"></asp:Literal>
                                </div>
                                <asp:GridView ID="GridViewCDCP" Width="850px" AutoGenerateColumns="true" OnRowDataBound="gv_RowDataBound" runat="server">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="false" CssClass="sCabecera_principal" />
                                    <RowStyle HorizontalAlign="Center" Wrap="false" CssClass="sItemT" />
                                </asp:GridView>
                            </div>


                            <div class="tab-pane fade" id="profileCD">
                                <div style="text-align: center; height: 400px; width: 840px">
                                    <asp:Literal ID="Panel_CD_Zoom_LP" runat="server"></asp:Literal>
                                </div>
                                <asp:GridView ID="GridViewCDLP" Width="850px" AutoGenerateColumns="true" OnRowDataBound="gv_RowDataBound" runat="server">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="false" CssClass="sCabecera_principal" />
                                    <RowStyle HorizontalAlign="Center" Wrap="false" CssClass="sItemT" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                    <div class="box-corner1"></div>
                    <div class="box-corner2"></div>

                </div>
            </div>


        </div>

        <%---Depositos--%>
        <div id="myModalDP" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" style="width: 900px; height: 720px;">
                <div class="modal-content">

                    <div class="topborder c2"></div>

                    <div class="modal-header">
                        <div class="box-header">
                            <h4>Depósitos (MM S/)</h4>
                            <img src="img/exit-modal.png" class="btn-exit-modal" data-dismiss="modal" />
                        </div>
                    </div>

                    <div class="modal-body">

                        <ul id="myTab1" class="ingresos nav nav-tabs">
                            <li class="active"><a href="#home1" data-toggle="tab">Soles</a></li>
                            <li><a href="#profile1" data-toggle="tab">Dólares</a></li>
                        </ul>

                        <div id="myTabContentDP" class="tab-content">


                            <div class="tab-pane fade in active" id="home1">
                                <div style="text-align: center; height: 400px; width: 840px">
                                    <asp:Literal ID="LiteralDepositosUsol" runat="server"></asp:Literal>
                                </div>
                                <asp:GridView ID="gvDepositosUsol" AutoGenerateColumns="False" runat="server" Width="840px">
                                    <Columns>


                                        <asp:BoundField DataField="INDICADOR" HeaderText="">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sCabecera_principal" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB1" HeaderText="JUN-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB2" HeaderText="JUL-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB3" HeaderText="AGO-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB4" HeaderText="SEP-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB5" HeaderText="OCT-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB6" HeaderText="NOV-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB7" HeaderText="DIC-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB8" HeaderText="ENE-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB9" HeaderText="FEB-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB10" HeaderText="MAR-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB11" HeaderText="ABR-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB12" HeaderText="MAY-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB13" HeaderText="JUN-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                    </Columns>

                                </asp:GridView>
                            </div>


                            <div class="tab-pane fade" id="profile1">
                                <div style="text-align: center; height: 400px; width: 840px">
                                    <asp:Literal ID="LiteralDepositosUs" runat="server"></asp:Literal>
                                </div>
                                <asp:GridView ID="gvDepositosUs" runat="server" AutoGenerateColumns="False" Width="840px">
                                    <Columns>
                                        <asp:BoundField DataField="INDICADOR" HeaderText="">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sCabecera_principal" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB1" HeaderText="JUN-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB2" HeaderText="JUL-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB3" HeaderText="AGO-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB4" HeaderText="SEP-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB5" HeaderText="OCT-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB6" HeaderText="NOV-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB7" HeaderText="DIC-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB8" HeaderText="ENE-16">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB9" HeaderText="FEB-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB10" HeaderText="MAR-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB11" HeaderText="ABR-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB12" HeaderText="MAY-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="CAB13" HeaderText="JUN-17">
                                            <HeaderStyle CssClass="sCabecera_principal" />
                                            <ItemStyle CssClass="sItemT" />
                                        </asp:BoundField>
                                    </Columns>


                                </asp:GridView>
                            </div>


                        </div>
                    </div>

                    <div class="box-corner1"></div>
                    <div class="box-corner2"></div>

                </div>
            </div>
        </div>


        <%--Contingentes--%>
        <div id="myModalCT" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="topborder c2"></div>

                    <div class="modal-header">
                        <div class="box-header">
                            <h4>Contingentes (MM S/)</h4>
                        </div>
                        <img src="img/exit-modal.png" class="btn-exit-modal" data-dismiss="modal" />
                    </div>

                    <div class="ingresos modal-body">
                        <div style="text-align: center; height: 400px; width: 840px">
                            <asp:Literal ID="LiteralContingente" runat="server"></asp:Literal>
                        </div>
                        <asp:GridView ID="GridViewCT" Width="850px" AutoGenerateColumns="True" OnRowDataBound="gv_RowDataBound" runat="server">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" CssClass="sCabecera_principal" />
                            <RowStyle HorizontalAlign="Center" Wrap="false" CssClass="sItemT" />
                        </asp:GridView>
                    </div>

                    <div class="box-corner1"></div>
                    <div class="box-corner2"></div>
                </div>
            </div>
        </div>

        <%--Ingresos Totales--%>
        <div id="myModalIT" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="topborder c2"></div>

                    <div class="modal-header">
                        <div class="box-header">
                            <h4>Ingresos Totales (M S/)</h4>
                        </div>
                        <img src="img/exit-modal.png" class="btn-exit-modal" data-dismiss="modal" />
                    </div>

                    <div class="ingresos modal-body">
                        <div style="text-align: center; height: 400px; width: 840px">
                            <asp:Literal ID="LiteralIT" runat="server"></asp:Literal>
                        </div>
                        <asp:GridView ID="GridViewIT" Width="850px" AutoGenerateColumns="True" OnRowDataBound="gv_RowDataBound" runat="server">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" CssClass="sCabecera_principal" />
                            <RowStyle HorizontalAlign="Center" Wrap="false" CssClass="sItemT" />
                        </asp:GridView>
                    </div>

                    <div class="box-corner1"></div>
                    <div class="box-corner2"></div>
                </div>
            </div>
        </div>

        <%--Distribucion IXS--%>
        <div id="myModalIXS" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="topborder c2"></div>

                    <div class="modal-header">
                        <div class="box-header">
                            <h4>Distribución de ingresos (M S/)</h4>
                        </div>
                        <img src="img/exit-modal.png" class="btn-exit-modal" data-dismiss="modal" />
                    </div>

                    <div class="ingresos modal-body">
                        <div style="text-align: center; height: 400px; width: 840px">
                            <asp:Literal ID="LiteralIXS" runat="server"></asp:Literal>
                        </div>
                        <asp:GridView ID="GridViewIXS" Width="850px" AutoGenerateColumns="True" OnRowDataBound="gv_RowDataBound" runat="server">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" CssClass="sCabecera_principal" />
                            <RowStyle HorizontalAlign="Center" Wrap="false" CssClass="sItemT" />
                        </asp:GridView>
                    </div>

                    <div class="box-corner1"></div>
                    <div class="box-corner2"></div>
                </div>
            </div>
        </div>

        <%--Distribucion PDM--%>
        <div id="myModalDistPDM" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="topborder c2"></div>

                    <div class="modal-header">
                        <div class="box-header">
                            <h4>Trend de PDM (MM S/)</h4>
                        </div>
                        <img src="img/exit-modal.png" class="btn-exit-modal" data-dismiss="modal" />
                    </div>

                    <div class="ingresos modal-body">
                        <div style="text-align: center; height: 400px; width: 840px">
                            <asp:Literal ID="LiteralDPDM" runat="server"></asp:Literal>
                        </div>
                        <asp:GridView ID="GridViewDPDM" Width="850px" AutoGenerateColumns="True" OnRowDataBound="gv_RowDataBound" runat="server">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" CssClass="sCabecera_principal" />
                            <RowStyle HorizontalAlign="Center" Wrap="false" CssClass="sItemT" />
                        </asp:GridView>

                        <div class="box-corner1"></div>
                        <div class="box-corner2"></div>
                    </div>
                </div>
            </div>
        </div>

        <%--Trend PDM--%>
        <div id="myModalTrendPDM" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="topborder c2"></div>

                    <div class="modal-header">
                        <div class="box-header">
                            <h4>Trend de PDM (MM S/)</h4>
                        </div>
                        <img src="img/exit-modal.png" class="btn-exit-modal" data-dismiss="modal" />
                    </div>

                    <div class="ingresos modal-body">
                        <div style="text-align: center; height: 400px; width: 840px">
                            <asp:Literal ID="LiteralTPDM" runat="server"></asp:Literal>
                        </div>
                        <asp:GridView ID="GridViewPDM" Width="850px" AutoGenerateColumns="True" OnRowDataBound="gv_RowDataBound" runat="server">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" CssClass="sCabecera_principal" />
                            <RowStyle HorizontalAlign="Center" Wrap="false" CssClass="sItemT" />
                        </asp:GridView>

                        <div class="box-corner1"></div>
                        <div class="box-corner2"></div>
                    </div>
                </div>
            </div>
        </div>

        <%--UsoProductosZoom--%>
        <div id="myModalUsoProducto" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="topborder c3"></div>

                    <div class="modal-header">
                        <div class="box-header">
                            <h4>Uso de Productos</h4>
                            <img src="img/exit-modal.png" class="btn-exit-modal" data-dismiss="modal" />
                        </div>
                    </div>

                    <div class="modal-body">
                        <ul id="tabsUsoProducto" class="usoproductos nav nav-tabs">
                            <li class="active"><a href="#ma" data-toggle="tab">MARGEN ACTIVO</a></li>
                            <li><a href="#mp" data-toggle="tab">MARGEN PASIVO</a></li>
                            <li><a href="#cmx" data-toggle="tab">COMEX</a></li>
                            <li><a href="#tsr" data-toggle="tab">TESORERIA</a></li>
                            <li><a href="#otr" data-toggle="tab">OTROS</a></li>
                            <%--<li><a href="#all" data-toggle="tab">TODOS</a></li>--%>
                        </ul>
                        <div id="contenidosUsoProducto" class="tab-content" style="text-align: center; height: 400px; width: 100%; margin: 0px auto;">
                            <%--1. Margen Activo (Mg. Col. + Mg. Der. + IxS Asoc.)--%>
                            <div class="tab-pane fade in active" id="ma">
                                <div class="panel-group" id="accordion_ma">
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_ma" href="#accordion_ma_1">COMERCIO EXTERIOR
                                            </a>
                                        </h4>
                                        <div id="accordion_ma_1" class="panel-collapse collapse in">
                                            <div class="panel-body">
                                                <asp:Table ID="tM1_COMEX" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_ma" href="#accordion_ma_2">PRESTAMOS COMERCIALES
                                            </a>
                                        </h4>
                                        <div id="accordion_ma_2" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <asp:Table ID="tM1_PRECOM" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_ma" href="#accordion_ma_3">FINANZAS CORPORATIVAS
                                            </a>
                                        </h4>
                                        <div id="accordion_ma_3" class="panel-collapse collapse">
                                            <div class="panel-body">

                                                <asp:Table ID="tM1_FINCOR" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_ma" href="#accordion_ma_4">OTROS (TARJETAS, BONOS CORP. Y CRÉDITOS)
                                            </a>
                                        </h4>
                                        <div id="accordion_ma_4" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <asp:Table ID="tM1_OTROS" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--2. Margen Pasivo (Mg. Depósitos Vista + IxS Netos Asociados (Recaudación + Cta Cte + AFP + SUNAT))--%>
                            <div class="tab-pane fade" id="mp">
                                <div class="panel-group" id="accordion_mp">
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_mp" href="#accordion_mp_1">CUENTA CORRIENTE
                                            </a>
                                        </h4>
                                        <div id="accordion_mp_1" class="panel-collapse collapse in">
                                            <div class="panel-body">
                                                <asp:Table ID="tM2_CTACTE" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_mp" href="#accordion_mp_2">RECAUDACIONES Y PAGOS
                                            </a>
                                        </h4>
                                        <div id="accordion_mp_2" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <asp:Table ID="tM2_RECPAG" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_mp" href="#accordion_mp_3">OTROS (CBME, AHORROS Y PLAZO MIAMI)
                                            </a>
                                        </h4>
                                        <div id="accordion_mp_3" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <asp:Table ID="tM2_OTROS" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--3.Comex--%>
                            <div class="tab-pane fade" id="cmx">
                                <div class="panel-group" id="accordion_cmx">
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_cmx" href="#accordion_cmx_1">COMERCIO EXTERIOR
                                            </a>
                                        </h4>
                                        <div id="accordion_cmx_1" class="panel-collapse collapse in">
                                            <div class="panel-body">
                                                <asp:Table ID="tM3_COMEX" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--4. Tesoreria--%>
                            <div class="tab-pane fade" id="tsr">
                                <div class="panel-group" id="accordion_tsr">
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_tsr" href="#accordion_tsr_1">TESORERIA
                                            </a>
                                        </h4>
                                        <div id="accordion_tsr_1" class="panel-collapse collapse in">
                                            <div class="panel-body">
                                                <asp:Table ID="tM4_TESO" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--5. Otros (IxS Netos)--%>
                            <div class="tab-pane fade" id="otr">
                                <div class="panel-group" id="accordion_otr">
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_otr" href="#accordion_otr_1">COBRANZAS
                                            </a>
                                        </h4>
                                        <div id="accordion_otr_1" class="panel-collapse collapse in">
                                            <div class="panel-body">
                                                <asp:Table ID="tM5_COBR" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_otr" href="#accordion_otr_2">CONTINGENTES
                                            </a>
                                        </h4>
                                        <div id="accordion_otr_2" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <asp:Table ID="tM5_CONT" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_otr" href="#accordion_otr_3">GIROS Y TRANSFERENCIAS
                                            </a>
                                        </h4>
                                        <div id="accordion_otr_3" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <asp:Table ID="tM5_GGTT" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_otr" href="#accordion_otr_4">TELECREDITO
                                            </a>
                                        </h4>
                                        <div id="accordion_otr_4" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <asp:Table ID="tM5_RECPAG" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <h4 class="panel-title">
                                            <a role="button" data-toggle="collapse" data-parent="#accordion_otr" href="#accordion_otr_5">OTROS (FONDOS MUTUOS, NNFF)
                                            </a>
                                        </h4>
                                        <div id="accordion_otr_5" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <asp:Table ID="tM5_OTROS" runat="server">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%-- <div class="tab-pane fade" id="all">
                                <div class="panel-body" style="max-height: 415px; overflow: auto;">
                                    <asp:Table ID="tTODOS" runat="server">
                                    </asp:Table>
                                </div>
                            </div>--%>
                        </div>
                    </div>

                    <div class="box-corner1"></div>
                    <div class="box-corner2"></div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
