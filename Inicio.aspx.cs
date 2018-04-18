using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FusionCharts.Charts;
using BCP.PERCLI.BUS.BEL;
using BCP.PERCLI.BUS.BLL;
using System.Data;

namespace BCP.PERCLI.GUI.WEB
{

    public partial class Inicio : System.Web.UI.Page
    {
        public static string RARORACpercent { get; set; }
        DatoUsuario objUsuario = new DatoUsuario();
        String pUsuarioActual = String.Empty;
        DataSet ds = default(DataSet);
        DataTable dt = new DataTable();
        DataView dv = new DataView();
        DataTable dtCloned = new DataTable();
        DatoCartera objCartera = new DatoCartera();
        DataTable dataCliente = new DataTable();
        public StringBuilder validacion;

        /*******Load*************/
        //Filtra clientes segun usuario
        protected void Page_Init(object sender, EventArgs e)
        {
            //System.Security.Principal.WindowsIdentity currentUser = System.Security.Principal.WindowsIdentity.GetCurrent();
            //pUsuarioActual = currentUser.Name.Substring(currentUser.Name.Length - 6).ToUpper();
            //ds = ListaDatoClienteBL.instancia.RCLI_OBT_LISTADATOCLIENTE(pUsuarioActual);
            //dt = ds.Tables[0];
            //Session["dt_Cliente"] = dt.Copy();
            //AQUI TRAE DATA SET, lo guardamos en una lista y luego esa lista la colocamos en fuct1
        }
        //Carga el panel de grafica
        protected void Page_Load(object sender, EventArgs e)
        {
            string idc = Request.QueryString["IDC"] as string;
            if (idc != null)
            {
                System.Security.Principal.WindowsIdentity currentUser = System.Security.Principal.WindowsIdentity.GetCurrent();
                pUsuarioActual = currentUser.Name.Substring(currentUser.Name.Length - 6).ToUpper();
                ds = ListaDatoClienteBL.instancia.RCLI_OBT_LISTADATOCLIENTE(pUsuarioActual);
                dt = ds.Tables[0];
                Session["dt_Cliente"] = dt.Copy();
                txt_cliente.Text = idc;
                btn_buscar_indicador(sender, e);
            }
            else
            {
                System.Security.Principal.WindowsIdentity currentUser = System.Security.Principal.WindowsIdentity.GetCurrent();
                pUsuarioActual = currentUser.Name.Substring(currentUser.Name.Length - 6).ToUpper();
                ds = ListaDatoClienteBL.instancia.RCLI_OBT_LISTADATOCLIENTE(pUsuarioActual);
                dt = ds.Tables[0];
                Session["dt_Cliente"] = dt.Copy();
                objUsuario = GestionUsuarioBL.instancia.AutenticarUsuario(pUsuarioActual);
                ValidacionUsuarios(objUsuario);
            } if (!IsPostBack)
            {
                CargarSectorEconomico();
                MostrarArchivosMacro();
                MostrarTop4Semanales();
                String IDCliente = Session["IDCliente"] as String;
                if (!String.IsNullOrEmpty(IDCliente))
                {
                    txt_cliente.Text = IDCliente;
                    btn_buscar_indicador(null, null);
                }
            }


        }

        public void ValidacionUsuarios(DatoUsuario objUsuario)
        {
            //No hay validacion para la página de Inicio
        }

        /*******Cliente*************/
        //Listar Cliente
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod()]
        //Listar clientes en el TextBox
        public static String[] ListarClientes_Reporte(String prefix)
        {
            DataTable dt = null;
            try
            {
                dt = System.Web.HttpContext.Current.Session["dt_Cliente"] as DataTable;
            }
            catch (Exception e)
            {
                Console.WriteLine("Revisar el metodo Reporte", e.Source);
            }

            //Agregar a lista desplegable
            List<String> clientes = new List<String>();
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                clientes.Add(String.Format("{0}//{1}", dt.Rows[i]["CLIENTE"].ToString().Trim() + " " + dt.Rows[i]["IDC"].ToString().Trim(), dt.Rows[i]["CLIENTE"].ToString().Trim()));
            }

            //Elimina duplicados
            IEnumerable<String> distinctAges = clientes.Distinct();

            //Inserta a una nueva lista de clientes
            List<String> clientes2 = new List<String>();
            foreach (String d in distinctAges)
            {
                clientes2.Add(d);
            }

            //Filtrar solo el top 10 de la búsqueda
            List<String> clientes3 = new List<String>();
            List<String> clientesFinal = new List<String>();
            IEnumerable<String> distinctAges2 = clientes2.Where(f => f.ToLower().IndexOf(prefix.ToLower()) != -1).ToArray();

            clientes3 = distinctAges2.ToList();
            int size = clientes3.Count;

            for (int i = 0; i < 5; i++)
            {
                if (i < size)
                {
                    clientesFinal.Add(clientes3[i]);
                }
            }

            return clientesFinal.ToArray();
        }
        //Buscar Cliente
        protected void btn_buscar_indicador(object sender, EventArgs e)
        {
            dt = Session["dt_Cliente"] as DataTable;

            //Busqueda de clientes
            if (!String.IsNullOrEmpty(txt_cliente.Text))
            {
                dv = dt.DefaultView;
                dv.RowFilter = "[CLIENTE] = '" + txt_cliente.Text + "' OR [IDC] = '" + txt_cliente.Text + "'";

                if (dv.ToTable().Rows.Count == 0)
                {
                    //Message
                    validacion = new StringBuilder();
                    validacion.Append("Por favor, seleccionar un cliente de la lista o ingresar un cliente de su cartera.");

                    //Advertencia                   
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "Validacion_Solicitud('Bienvenido a la Web Resumen Cliente');", true);
                }
                else
                {

                    dataCliente = dv.ToTable();

                    int codclavecic = Convert.ToInt32(dataCliente.Rows[0]["CODCLAVECIC"]);

                    Session["IDCliente"] = txt_cliente.Text;
                    Session["IDC"] = dataCliente.Rows[0]["IDC"].ToString();
                    Session["CODUNICOCLI"] = dataCliente.Rows[0]["CODUNICOCLI"].ToString();


                    MostrarArchivosSegunCIIU(dataCliente);

                    //IndicadorFile
                    GraficaIndicadorFile(codclavecic);

                    //Trend RARORAC
                    GraficaTrendRARORAC(codclavecic);

                    //Colocaciones
                    GraficaColocacionDirecta("mscombidy2d", "Ult12mAnt", "Ult12m", "Spread 16-17", "", codclavecic);
                    GraficaColocacionDirecta_Zoom("mscombidy2d", "Ult12mAnt", "Ult12m", "Spread 16-17", "", codclavecic, "CP", Panel_CD_Zoom_CP, Panel_CD_Zoom_CP_Print, GridViewCDCP);
                    GraficaColocacionDirecta_Zoom("mscombidy2d", "Ult12mAnt", "Ult12m", "Spread 16-17", "", codclavecic, "LP", Panel_CD_Zoom_LP, Panel_CD_Zoom_LP_Print, GridViewCDLP);

                    //Contingentes
                    GraficaContingente("mscombidy2d", "Ult12mAnt", "Ult12m", "Com. 16-17", "", codclavecic);
                    GraficaContingete_Zoom("mscombidy2d", "Ult12mAnt", "Ult12m", "Com. 16-17", "", codclavecic, GridViewCT);

                    //Depositos
                    GraficaDeposito("mscombidy2d", "Ult12mAnt", "Ult12m", "Spread 16-17", "", codclavecic);
                    GraficaDepositoZoom("msstackedcolumn2dlinedy", "Real VR Año Ant.", "Real VNR Año Ant.", "Real VR Ult. Año", "Real VNR Ult. Año", "Spread VR", "Spread VNR", codclavecic, 0, LiteralDepositosUsol, LiteralDepositosUsol_Print, gvDepositosUsol);
                    GraficaDepositoZoom("msstackedcolumn2dlinedy", "Real VR Año Ant.", "Real VNR Año Ant.", "Real VR Ult. Año", "Real VNR Ult. Año", "Spread VR", "Spread VNR", codclavecic, 2, LiteralDepositosUs, LiteralDepositosUs_Print, gvDepositosUs);
                    GraficaPDHZoom("msstackedcolumn2dlinedy", "# Clientes", "Abono - PDH", "Real VR Ult. Año", "Real VNR Ult. Año", "Spread VR", "Spread VNR", Session["CODUNICOCLI"].ToString(), 0, LiteralDepositosUs1, LiteralDepositosUs_Print, gvCuentaSueldo);

                    //Ingresos Totales
                    GraficaIngresoTotal("StackedColumn2D", "MA", "MP", "IXS", "COSTO", codclavecic);
                    GraficaIngresoTotal_Zoom("StackedColumn2D", "MA", "MP", "IXS", "COSTO", codclavecic);

                    //Distribucion de Ingresos                    
                    GraficaDistribucionIngreso("MSLine", codclavecic);
                    GraficaDistribucionIngreso_Zoom("MSLine", codclavecic);

                    //Matriz de uso
                    GraficaUsoProducto(codclavecic);
                    GraficaUsoProducto_Zoom(codclavecic);

                    //Dist PDM
                    GraficaDistribucionPdm("Radar", "BCP", "SBS", codclavecic);
                    GraficaDistribucionPdm_Zoom("Radar", "BCP", "SBS", codclavecic);

                    //TrendPDM
                    GraficaTrenPdm("mscombidy2d", codclavecic);
                    GraficaTrenPdm_Zoom("mscombidy2d", codclavecic);

                    lblClient.Text = "";
                    lblClientType.Text = "";
                    lblRating.Text = "";
                    lblRisk.Text = "";
                    lblMaxCEM.Text = "";
                    lblUtilCEM.Text = "";
                    lblRARORAC.Text = "";
                    lblCuadrante.Text = "";
                    lblEmpleados.Text = "";

                    string razonSocial = dataCliente.Rows[0]["CLIENTE"].ToString().Trim();

                    lblClient.Text = (razonSocial.Length>35)?razonSocial.Substring(0,30)+".....":razonSocial;
                    lblClientType.Text = dataCliente.Rows[0]["ESTRATEGICO"].ToString();
                    lblRating.Text = dataCliente.Rows[0]["RATING"].ToString();
                    lblRisk.Text = dataCliente.Rows[0]["RIESGOSBS"].ToString();
                    lblMaxCEM.Text = "CEM Max (S/): " + String.Format("{0:0,0.0}", Convert.ToDouble(dataCliente.Rows[0]["MTOMAXIMOCEM"].ToString()));
                    lblUtilCEM.Text = "CEM Util (S/): " + String.Format("{0:0,0.0}", Convert.ToDouble(lblUtilCEM.Text + dataCliente.Rows[0]["MTOUTILIZADOCEM"].ToString()));
                    lblCuadrante.Text = "CUADRANTE MATRIZ COMERCIAL: " + dataCliente.Rows[0]["CC_BANCA"].ToString();
                    lblEmpleados.Text = "#EMPLEADOS TOTAL: " + FormatoNumDecimal(GestionRevalidacionCEMBL.instancia.ObtenerTotalEmpleados(Session["CODUNICOCLI"].ToString()),0,1);

                    Double RARORACCliente = 0;
                    Double RARORACGA = 0;
                    Double Cumplimiento = 0;
                    Double CumplimientoLinea = 0;

                    RARORACCliente = Convert.ToDouble(dataCliente.Rows[0]["IN_RARORAC"].ToString()) * 100;
                    RARORACGA = Convert.ToDouble(dataCliente.Rows[0]["PPTORARORACGA"].ToString()) * 100;

                    Cumplimiento = (RARORACCliente / RARORACGA) * 100;

                    if (Cumplimiento >= 100)
                        CumplimientoLinea = 120;
                    else
                        CumplimientoLinea = Cumplimiento;

                    lblRARORAC.Text = String.Format("{0:0.0}%", RARORACCliente);
                    ppto_RARORACGA.InnerText = String.Format("{0:0.0}%", RARORACGA);

                    boxbodyRARORAC.Style.Add("display", "block");

                    realRARORAC.Attributes.Remove("data-fill");
                    realRARORAC.Attributes.Add("data-fill", String.Format("{0:0}", CumplimientoLinea.ToString()));
                    cumpRarorac.Text = "Cump: " + String.Format("{0:0}%", Cumplimiento);





                    //if (Cumplimiento >= 61)
                    //    cumpRARORAC.Style.Add("top", "33px");
                    //else if (Cumplimiento >= 40 && Cumplimiento <= 60)
                    //    cumpRARORAC.Style.Add("top", "103px");
                    //else if (Cumplimiento <= 39)
                    //    cumpRARORAC.Style.Add("top", "173px");


                    ListaDatoClienteBL raroracCartera = new ListaDatoClienteBL();

                    objCartera = raroracCartera.RCLI_OBT_RARORACxSECTOR(dataCliente.Rows[0]["CODSECTOR"].ToString());
                    realCARTERA.Text = String.Format("{0:0.0}%", objCartera.RARORAC * 100);




                    /*impresion*/

                    ppto_RARORACGA_resumen.Text = ppto_RARORACGA.InnerText.ToString(); ;
                    cumpRarorac_resumen.Text = String.Format("{0:0}%", Cumplimiento);
                    lblRARORAC_resumen.Text = lblRARORAC.Text;
                    realCARTERA_resumen.Text = realCARTERA.Text;

                    /*impresion*/

                    ppto_RARORACGA_detalle.Text = ppto_RARORACGA.InnerText.ToString(); ;
                    cumpRarorac_detalle.Text = String.Format("{0:0}%", Cumplimiento);
                    lblRARORAC_detalle.Text = lblRARORAC.Text;
                    realCARTERA_detalle.Text = realCARTERA.Text;



                    Session["CODSECTORCliente"] = dataCliente.Rows[0]["CODSECTOR"].ToString();

                    hlFDG.NavigateUrl = "http://10.80.218.93:9007/Views/Reporte/IndicadoresFile.aspx?LLAVEFILE=" + dataCliente.Rows[0]["CODSECTOR"].ToString();
                }
            }
            else
            {
                //Message
                validacion = new StringBuilder();
                validacion.Append("Por favor, seleccionar un cliente de la lista o ingresar un cliente de su cartera.");

                //Advertencia                   
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "Validacion_Solicitud('Bienvenido a la Web Resumen Cliente');", true);
            }

        }
        protected void btn_download_pdf(object sender, EventArgs e)
        {
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=GUÍA_RESUMEN_CLIENTE.pdf");
            Response.TransmitFile(Server.MapPath("~/pdf/GUÍA_RESUMEN_CLIENTE.pdf"));
            Response.End();
        }

        /*************Graficas-Primera FIla*************/
        //Indicadores File
        protected void GraficaIndicadorFile(Int32 iCodclavecic)
        {


            ds = IndicadorFileBL.instancia.RCLI_OBT_INDICADORFILE(iCodclavecic);

            DataTable dt_tiempo = ds.Tables[1];
            Session["dt_tiempo_indicadores_file"] = dt_tiempo;

            gvData1.DataSource = ds.Tables[0];
            gvData1.DataBind();


        }
        //Trend de RARORAC
        protected void GraficaTrendRARORAC(Int32 iCodclavecic)
        {
            ds = TrendRaroracBL.instancia.RCLI_OBT_GDV_RARORAC(iCodclavecic);

            DataTable dt_tiempo = ds.Tables[1];

            Session["dt_tiempo_TrendRarorac"] = dt_tiempo;

            dgvTrendRARORAC.DataSource = ds.Tables[0];
            dgvTrendRARORAC.DataBind();
        }

        /*************Graficas-Segunda FIla*************/
        //Colocaciones
        protected void GraficaColocacionDirecta(String sDisenio, String sSerieNombre1, String sSerieNombre2, String sSerieNombre3, String sSerieNombre4, int codclavecic)
        {

            ds = ColocacionDirectaBL.instancia.RCLI_OBT_COLOCACIONDIRECTA(codclavecic);
            dt = ds.Tables[0];

            Session["dt_Colocacion_Zoom"] = ds.Tables[0];
            Session["dt_Colocacion_Gridview_CP"] = ds.Tables[1];
            Session["dt_Colocacion_Gridview_LP"] = ds.Tables[2];
            Session["dt_Colocacion_Tiempo"] = ds.Tables[3];

            object[,] arrData = new object[12, 4];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 6; i++)
            {
                arrData[i, 0] = ds.Tables[3].Rows[i + 6][1].ToString();
                //if (ds.Tables[3].Rows[i + 6][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            int total = dt.Rows.Count;
            int Smeses = total - 6;

            for (int i = 0; i < 6; i++)
            {

                arrData[i, 1] = dt.Rows[i + 6][2].ToString(); //anterior                
                arrData[i, 2] = dt.Rows[i + Smeses][2].ToString();//actual
                arrData[i, 3] = dt.Rows[i + Smeses][4].ToString();//spread
            }

            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/

            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/  
                    "'showValues': '0'," +
                    "'anchorRadius': '6'," +
                    "'lineThickness': '3'," +
                    "'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                    "'paletteColors': '#2E75B6,#FF4F00,#FFC000'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + sSerieNombre1 + "'");
            serieB.Append("{'seriesname': '" + sSerieNombre2 + "', 'alpha': '75' ");
            serieC.Append("{'seriesname': '" + sSerieNombre3 + "', 'parentYAxis': 'S','renderAs': 'line'");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CUERPO*/
            for (int i = 0; i < 6; i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieC.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");

            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartCD", "100%", "85%", "json", jsonData.ToString());
            Panel_CD.Text = sales.Render();

            Chart sales_print_resumen = new Chart(sDisenio, "myChartCD_print_resumen", "350px", "200px", "json", jsonData.ToString());
            Panel_CD_print_resumen.Text = sales_print_resumen.Render();

            Chart sales_print_detalle = new Chart(sDisenio, "myChartCD_print_detalle", "540px", "240px", "json", jsonData.ToString());
            Panel_CD_print_detalle.Text = sales_print_detalle.Render();
        }
        protected void GraficaColocacionDirecta_Zoom(String sDisenio, String sSerieNombre1, String sSerieNombre2, String sSerieNombre3, String sSerieNombre4, int codclavecic, string plazo, Literal lGrafico, Literal lGraficoPrint, GridView gvTabla)
        {

            string MyChart = "ChartCD" + plazo;

            dt = Session["dt_Colocacion_Zoom"] as DataTable;

            DataTable dt1, dt2, dt3 = new DataTable();
            dt3 = Session["dt_Colocacion_Tiempo"] as DataTable;

            object[,] arrData = new object[24, 7];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();

            for (int i = 0; i < 12; i++)
            {
                arrData[i, 0] = dt3.Rows[i][1].ToString();
            }

            //Store sales data for the current year in the second column.
            int total = dt.Rows.Count;
            int Smeses = total - 12;

            if (plazo == "CP")
            {
                for (int i = 0; i < 12; i++)
                {
                    arrData[i, 1] = dt.Rows[i][5].ToString();
                    arrData[i, 2] = dt.Rows[i + Smeses][5].ToString();
                    arrData[i, 3] = dt.Rows[i + Smeses][7].ToString();
                }

                dt1 = Session["dt_Colocacion_Gridview_CP"] as DataTable;

                GridViewCDCP.DataSource = tablaFormato_gv_Reporte(dt1);
                GridViewCDCP.DataBind();
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    arrData[i, 1] = dt.Rows[i][8].ToString();
                    arrData[i, 2] = dt.Rows[i + Smeses][8].ToString();
                    arrData[i, 3] = dt.Rows[i + Smeses][10].ToString();
                }

                dt2 = Session["dt_Colocacion_Gridview_LP"] as DataTable;

                GridViewCDLP.DataSource = tablaFormato_gv_Reporte(dt2);
                GridViewCDLP.DataBind();
            }

            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas
            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/

            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/
                    "'showValues': '0'," +
                    "'anchorRadius': '6'," +
                    "'lineThickness': '3'," +
                    "'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                    "'paletteColors': '#2E75B6,#FF4F00,#FFC000'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + sSerieNombre1 + "'");
            serieB.Append("{'seriesname': '" + sSerieNombre2 + "', 'alpha': '75' ");
            serieC.Append("{'seriesname': '" + sSerieNombre3 + "', 'parentYAxis': 'S','renderAs': 'line'");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CUERPO*/
            for (int i = 0; i < 12; i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieC.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, MyChart, "100%", "400px", "json", jsonData.ToString());
            lGrafico.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, MyChart + "_Print", "540px", "220px", "json", jsonData.ToString());
            lGraficoPrint.Text = sales_print.Render();
        }

        //Depositos
        protected void GraficaDeposito(String sDisenio, String sSerieNombre1, String sSerieNombre2, String sSerieNombre3, String sSerieNombre4, int codclavecic)
        {
            ds = DepositoBL.instancia.RCLI_OBT_DEPOSITO(codclavecic);
            dt = ds.Tables[0];

            object[,] arrData = new object[6, 4];
            //Store product labels in the first column.
            //arrData[0, 0] = "MAR";
            //arrData[1, 0] = "ABR";
            //arrData[2, 0] = "MAY";
            //arrData[3, 0] = "JUN";
            //arrData[4, 0] = "JUL";
            //arrData[5, 0] = "AGO";




            for (int i = 0; i < 6; i++)
            {
                arrData[i, 0] = ds.Tables[1].Rows[i + 12][1].ToString();
            }

            //Store sales data for the current year in the second column.
            for (int i = 0; i < 6; i++)
            {
                arrData[i, 1] = dt.Rows[i][3].ToString();
                arrData[i, 2] = dt.Rows[i + 12][3].ToString();
                arrData[i, 3] = dt.Rows[i + 12][4].ToString();
            }

            //Store sales data for previous year in the third column.
            //arrData[0, 2] = 367;
            //arrData[1, 2] = 584;
            //arrData[2, 2] = 754;
            //arrData[3, 2] = 456;
            //arrData[4, 2] = 754;
            //arrData[5, 2] = 437;

            /*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            //arrData[0, 3] = 4.785;
            //arrData[1, 3] = 5.321;
            //arrData[2, 3] = 7.850;
            //arrData[3, 3] = 5.044;
            //arrData[4, 3] = 6.112;
            //arrData[5, 3] = 7.145;

            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/
                    "'showValues': '0'," +
                    "'anchorRadius': '6'," +
                    "'lineThickness': '3'," +
                    "'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                    "'paletteColors': '#2E75B6,#FF4F00,#FFC000'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'plotFillAlpha': '90'," +
                    "'placeValuesInside': '1'," +
                    "'decimals': '2'" +
                "},");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + sSerieNombre1 + "' ");
            serieB.Append("{'seriesname': '" + sSerieNombre2 + "', 'alpha': '75'");
            serieC.Append("{'seriesname': '" + sSerieNombre3 + "', 'parentYAxis': 'S','renderAs': 'line'");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CUERPO*/
            for (int i = 0; i < arrData.GetLength(0); i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieC.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartDP", "100%", "85%", "json", jsonData.ToString());
            Panel_DP.Text = sales.Render();


            Chart sales_print_resumen = new Chart(sDisenio, "myChartDP_print_resumen", "350px", "200px", "json", jsonData.ToString());
            Panel_DP_print_resumen.Text = sales_print_resumen.Render();

            Chart sales_print_detalle = new Chart(sDisenio, "myChartDP_print_detalle", "540px", "240px", "json", jsonData.ToString());
            Panel_DP_print_detalle.Text = sales_print_detalle.Render();





        }
        protected void GraficaDepositoZoom(String sDisenio, String sSerieNombre1, String sSerieNombre2, String sSerieNombre3, String sSerieNombre4, String sSerieNombre5, String sSerieNombre6, int codclavecic, int iMoneda, Literal lGrafico, Literal lGraficoPrint, GridView gvTabla)
        {
            string MyChart = "ChartDPZ" + iMoneda.ToString() + lGrafico.ID.ToString();

            ds = DepositoBL.instancia.RCLI_OBT_DEPOSITO_ZOOM(codclavecic);
            dt = ds.Tables[iMoneda];

            DataTable dt_tiempo = ds.Tables[4];

            Session["dt_tiempo_depositos"] = dt_tiempo;

            gvTabla.DataSource = ds.Tables[iMoneda + 1];
            gvTabla.DataBind();




            object[,] arrData = new object[13, 5];



            for (int i = 0; i < 13; i++)
            {
                arrData[i, 0] = dt_tiempo.Rows[i][2].ToString();
            }

            for (int i = 0; i < 13; i++)
            {
                arrData[i, 1] = dt.Rows[i][2].ToString();
                arrData[i, 2] = dt.Rows[i][3].ToString();
                arrData[i, 3] = dt.Rows[i][5].ToString();
                arrData[i, 4] = dt.Rows[i][6].ToString();
            }

            StringBuilder jsonData = new StringBuilder();

            StringBuilder features = new StringBuilder();
            StringBuilder categories = new StringBuilder();

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder();
            StringBuilder serieD = new StringBuilder();


            /*CABEZA*/

            features.Append(
           "'chart': {" +

                "'sNumberSuffix': '%'," +
                 "'showValues': '0'," +
                "'anchorRadius': '6'," +
                "'lineThickness': '3'," +
                "'anchorBgColor': '#FFFFFF'," +
                "'paletteColors': '#FFC000,#D64200,#70AD47,#002060'," +
                "'baseFontColor': '#7F7F7F'," +
                "'showBorder': '0', " +
                "'showBorder': '0'," +
                "'bgColor': '#ffffff'," +
                "'showShadow': '0'," +
                "'canvasBgColor': '#ffffff'," +
                "'canvasBorderAlpha': '0'," +
                "'divlineAlpha': '100'," +
                "'divlineColor': '#999999'," +
                "'divlineThickness': '1'," +
                "'usePlotGradientColor': '0'," +
                "'showplotborder': '0'," +
                " 'showXAxisLine': '1'," +
                "'xAxisLineColor': '#999999'," +
                "'showAlternateHGridColor': '0'," +
                "'showAlternateVGridColor': '0'," +
                "'legendBgAlpha': '0'," +
                "'legendBorderAlpha': '0'," +
                "'legendItemFontSize': '8'," +
                "'formatNumberScale': '0'," +
                "'plotFillAlpha': '90'," +
                "'placeValuesInside': '1'," +
                "'decimals': '2'" +
             "}");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + sSerieNombre3 + "' ");// 'alpha': '75'");
            serieB.Append("{'seriesname': '" + sSerieNombre4 + "' ");// 'alpha': '75'");
            serieC.Append("{'seriesname': '" + sSerieNombre5 + "', 'parentYAxis': 'S','renderAs': 'line'");
            serieD.Append("{'seriesname': '" + sSerieNombre6 + "', 'parentYAxis': 'S','renderAs': 'line'");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            /*CUERPO*/
            for (int i = 0; i < arrData.GetLength(0); i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");
                    serieD.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");
                    serieD.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }

            /*PIE*/
            categories.Append("]}]");

            serieA.Append("]}");
            serieB.Append("]}");

            serieC.Append("]}");/*No olvidar que sigue otro grafico mas :D*/
            serieD.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/

            /*CIERRE*/
            jsonData.Append("{");
            jsonData.Append(features.ToString());
            jsonData.Append(",");
            jsonData.Append(categories.ToString());
            jsonData.Append(",");


            jsonData.Append("'dataset': [");
            jsonData.Append("{");

            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(",");
            jsonData.Append(serieB.ToString());
            jsonData.Append("]");

            jsonData.Append("}]");
            jsonData.Append(",");

            jsonData.Append("'lineset': [");
            jsonData.Append(serieC.ToString());
            jsonData.Append(",");
            jsonData.Append(serieD.ToString());
            jsonData.Append("]");
            jsonData.Append("}");

            /*RENDERIZAR*/

            Chart sales = new Chart(sDisenio, MyChart, "100%", "400px", "json", jsonData.ToString());
            lGrafico.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, MyChart + "_Print", "540px", "220px", "json", jsonData.ToString());
            lGraficoPrint.Text = sales_print.Render();

        }

       //Cuenta Sueldo
        protected void GraficaPDHZoom(String sDisenio, String sSerieNombre1, String sSerieNombre2, String sSerieNombre3, String sSerieNombre4, String sSerieNombre5, String sSerieNombre6, string idc, int iMoneda, Literal lGrafico, Literal lGraficoPrint, GridView gvTabla)
        {
            string MyChart = "ChartDPZ" + iMoneda.ToString() + lGrafico.ID.ToString();

            ds = DepositoBL.instancia.RCLI_OBT_PDH_ZOOM(idc);

            if (ds.Tables.Count == 3)
            {

                dt = ds.Tables[iMoneda];

                DataTable dt_tiempo = ds.Tables[1];

                Session["dt_tiempo_pdh"] = dt_tiempo;


                    gvCuentaSueldo.DataSource = ds.Tables[2];
                    gvCuentaSueldo.DataBind();




                    object[,] arrData = new object[13, 5];



                    for (int i = 0; i < 13; i++)
                    {
                        arrData[i, 0] = dt_tiempo.Rows[i][2].ToString();
                    }

                    for (int i = 0; i < 13; i++)
                    {
                        arrData[i, 1] = dt.Rows[i][3].ToString();//Linea Monto
                        arrData[i, 2] = dt.Rows[i][2].ToString();//Barra Numero Trabajadores
                        //arrData[i, 3] = dt.Rows[i][5].ToString();
                        //arrData[i, 4] = dt.Rows[i][6].ToString();
                    }

                    StringBuilder jsonData = new StringBuilder();

                    StringBuilder features = new StringBuilder();
                    StringBuilder categories = new StringBuilder();

                    StringBuilder serieA = new StringBuilder();
                    StringBuilder serieB = new StringBuilder();
                    //StringBuilder serieC = new StringBuilder();
                    //StringBuilder serieD = new StringBuilder();


                    /*CABEZA*/

                    features.Append(
                   "'chart': {" +
                        //"'sNumberSuffix': ' M'," +
                        "'sNumberPrefix': 'S/ '," +
                        "'showValues': '0'," +
                        "'anchorRadius': '6'," +
                        "'lineThickness': '3'," +
                        "'anchorBgColor': '#FFFFFF'," +
                        "'paletteColors': '#FFC000,#D64200,#70AD47,#002060'," +
                        "'baseFontColor': '#7F7F7F'," +
                        "'showBorder': '0', " +
                        "'showBorder': '0'," +
                        "'bgColor': '#ffffff'," +
                        "'showShadow': '0'," +
                        "'canvasBgColor': '#ffffff'," +
                        "'canvasBorderAlpha': '0'," +
                        "'divlineAlpha': '100'," +
                        "'divlineColor': '#999999'," +
                        "'divlineThickness': '1'," +
                        "'usePlotGradientColor': '0'," +
                        "'showplotborder': '0'," +
                        " 'showXAxisLine': '1'," +
                        "'xAxisLineColor': '#999999'," +
                        "'showAlternateHGridColor': '0'," +
                        "'showAlternateVGridColor': '0'," +
                        "'legendBgAlpha': '0'," +
                        "'legendBorderAlpha': '0'," +
                        "'legendItemFontSize': '8'," +
                        "'formatNumberScale': '0'," +
                        "'plotFillAlpha': '90'," +
                        "'placeValuesInside': '1'," +
                        "'decimals': '2'" +
                     "}");



                    categories.Append("'categories': [{'category': [");

                    serieA.Append("{'seriesname': '" + sSerieNombre1 + "' ");// 'alpha': '75'");
                    //serieB.Append("{'seriesname': '" + sSerieNombre4 + "' ");// 'alpha': '75'");
                    serieB.Append("{'seriesname': '" + sSerieNombre2 + "', 'parentYAxis': 'S','renderAs': 'line'");
                    //serieD.Append("{'seriesname': '" + sSerieNombre6 + "', 'parentYAxis': 'S','renderAs': 'line'");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                    /*CUERPO*/
                    for (int i = 0; i < arrData.GetLength(0); i++)
                    {
                        if (i == 0)
                        {
                            serieA.Append(",'data': [");
                            serieB.Append(",'data': [");
                            //    serieC.Append(",'data': [");
                            //    serieD.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                        }

                        if (i > 0)
                        {
                            categories.Append(",");
                            serieA.Append(",");
                            serieB.Append(",");
                            //serieC.Append(",");
                            //serieD.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                        }

                        categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                        serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                        serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                        //serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                        //serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                    }

                    /*PIE*/
                    categories.Append("]}]");

                    serieA.Append("]}");
                    serieB.Append("]}");

                    //serieC.Append("]}");/*No olvidar que sigue otro grafico mas :D*/
                    //serieD.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/

                    /*CIERRE*/
                    jsonData.Append("{");
                    jsonData.Append(features.ToString());
                    jsonData.Append(",");
                    jsonData.Append(categories.ToString());
                    jsonData.Append(",");


                    jsonData.Append("'dataset': [");
                    jsonData.Append("{");

                    jsonData.Append("'dataset': [");
                    jsonData.Append(serieA.ToString());
                    //jsonData.Append(",");
                    //jsonData.Append(serieB.ToString());
                    jsonData.Append("]");

                    jsonData.Append("}]");
                    jsonData.Append(",");

                    jsonData.Append("'lineset': [");
                    jsonData.Append(serieB.ToString());
                    //jsonData.Append(",");
                    //jsonData.Append(serieD.ToString());
                    jsonData.Append("]");
                    jsonData.Append("}");

                    /*RENDERIZAR*/

                    Chart sales = new Chart(sDisenio, MyChart, "100%", "400px", "json", jsonData.ToString());
                    lGrafico.Text = sales.Render();

                    Chart sales_print = new Chart(sDisenio, MyChart + "_Print", "540px", "220px", "json", jsonData.ToString());
                    lGraficoPrint.Text = sales_print.Render();
                
            }
        }

        //Contingentes
        protected void GraficaContingente(String sDisenio, String sSerieNombre1, String sSerieNombre2, String sSerieNombre3, String sSerieNombre4, int codclavecic)
        {
            ds = ContingenteBL.instancia.RCLI_OBT_CONTINGENTE(codclavecic);
            dt = ds.Tables[0];

            Session["dt_Contingentes_Zoom"] = ds.Tables[0];
            Session["dt_Contingentes_Gridview"] = ds.Tables[1];
            Session["dt_Contingentes_Tiempo"] = ds.Tables[2];

            object[,] arrData = new object[12, 4];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 6; i++)
            {
                arrData[i, 0] = ds.Tables[2].Rows[i + 6][1].ToString();
                //if (ds.Tables[2].Rows[i + 6][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            int total = dt.Rows.Count;
            int Smeses = total - 6;

            for (int i = 0; i < 6; i++)
            {
                arrData[i, 1] = dt.Rows[i + 6][3].ToString(); //anterior                
                arrData[i, 2] = dt.Rows[i + Smeses][3].ToString();//actual
                arrData[i, 3] = dt.Rows[i + Smeses][4].ToString();//spread
            }

            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/
                    "'showValues': '0'," +
                    "'anchorRadius': '6'," +
                    "'lineThickness': '3'," +
                    "'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                    "'paletteColors': '#2E75B6,#FF4F00,#FFC000'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + sSerieNombre1 + "'");
            serieB.Append("{'seriesname': '" + sSerieNombre2 + "', 'alpha': '75' ");
            serieC.Append("{'seriesname': '" + sSerieNombre3 + "', 'parentYAxis': 'S','renderAs': 'line'");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CUERPO*/
            for (int i = 0; i < 6; i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieC.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartCT", "100%", "85%", "json", jsonData.ToString());
            Panel_CT.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, "myChartCT_print", "350px", "200px", "json", jsonData.ToString());
            Panel_CT_print.Text = sales_print.Render();
        }
        protected void GraficaContingete_Zoom(String sDisenio, String sSerieNombre1, String sSerieNombre2, String sSerieNombre3, String sSerieNombre4, int codclavecic, GridView gvTabla)
        {

            //ds = ContingenteBL.instancia.RCLI_OBT_CONTINGENTE(codclavecic);
            //dt = ds.Tables[0];

            dt = Session["dt_Contingentes_Zoom"] as DataTable;

            DataTable dt1, dt2 = new DataTable();
            dt2 = Session["dt_Contingentes_Tiempo"] as DataTable;

            object[,] arrData = new object[24, 7];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 12; i++)
            {
                arrData[i, 0] = dt2.Rows[i][1].ToString();
                //if (dt2.Rows[i][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            int total = dt.Rows.Count;
            int Smeses = total - 12;

            for (int i = 0; i < 12; i++)
            {
                arrData[i, 1] = dt.Rows[i][3].ToString();
                arrData[i, 2] = dt.Rows[i + Smeses][3].ToString();
                arrData[i, 3] = dt.Rows[i + Smeses][4].ToString();
            }

            //dt1 = ds.Tables[1];
            dt1 = Session["dt_Contingentes_Gridview"] as DataTable;

            GridViewCT.DataSource = tablaFormato_gv_Reporte(dt1);
            GridViewCT.DataBind();


            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/
                    "'showValues': '0'," +
                    "'anchorRadius': '6'," +
                    "'lineThickness': '3'," +
                    "'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                    "'paletteColors': '#2E75B6,#FF4F00,#FFC000'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + sSerieNombre1 + "'");
            serieB.Append("{'seriesname': '" + sSerieNombre2 + "', 'alpha': '75' ");
            serieC.Append("{'seriesname': '" + sSerieNombre3 + "', 'parentYAxis': 'S','renderAs': 'line'");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CUERPO*/
            for (int i = 0; i < 12; i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieC.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartCT_Zoom", "100%", "400px", "json", jsonData.ToString());
            LiteralContingente.Text = sales.Render();


            Chart sales_print = new Chart(sDisenio, "myChartCT_Zoom_Print", "540px", "220px", "json", jsonData.ToString());
            LiteralContingente_Print.Text = sales_print.Render();
        }

        //Ingresos Totales
        protected void GraficaIngresoTotal(String sDisenio, String sSerieNombre1, String sSerieNombre2, String sSerieNombre3, String sSerieNombre4, int codclavecic)
        {
            ds = IngresoTotalBL.instancia.RCLI_OBT_INGRESOS_TOTALES(codclavecic);
            dt = ds.Tables[0];

            Session["dt_IT_Zoom"] = ds.Tables[0];
            Session["dt_IT_Gridview"] = ds.Tables[1];
            Session["dt_IT_Tiempo"] = ds.Tables[2];

            object[,] arrData = new object[12, 5];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 6; i++)
            {
                arrData[i, 0] = ds.Tables[2].Rows[i + 6][1].ToString();
                //if (ds.Tables[3].Rows[i + 6][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            int total = dt.Rows.Count;
            int Smeses = total - 6;

            for (int i = 0; i < 6; i++)
            {

                arrData[i, 1] = dt.Rows[i + Smeses][2].ToString(); //MA                
                arrData[i, 2] = dt.Rows[i + Smeses][3].ToString();//MP
                arrData[i, 3] = dt.Rows[i + Smeses][4].ToString();//IXS
                arrData[i, 4] = dt.Rows[i + Smeses][5].ToString();//COSTO
            }

            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder();
            StringBuilder serieD = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/

            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/  
                    "'showValues': '0'," +
                //"'anchorRadius': '6'," +
                    "'lineThickness': '3'," +
                //"'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                    "'paletteColors': '#FF4F00,#548235,#2E75B6,#9DC3E6'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + sSerieNombre1 + "'");
            serieB.Append("{'seriesname': '" + sSerieNombre2 + "', 'alpha': '75' ");
            serieC.Append("{'seriesname': '" + sSerieNombre3 + "'");
            serieD.Append("{'seriesname': '" + sSerieNombre4 + "', 'alpha': '75' ");


            /*CUERPO*/
            for (int i = 0; i < 6; i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");
                    serieD.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");
                    serieD.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");
            serieC.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieD.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());
            jsonData.Append(serieD.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartIT", "100%", "85%", "json", jsonData.ToString());
            Panel_IT.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, "myChartIT_print", "350px", "200px", "json", jsonData.ToString());
            Panel_IT_print.Text = sales_print.Render();
        }
        protected void GraficaIngresoTotal_Zoom(String sDisenio, String sSerieNombre1, String sSerieNombre2, String sSerieNombre3, String sSerieNombre4, int codclavecic)
        {
            //ds = ContingenteBL.instancia.RCLI_OBT_CONTINGENTE(codclavecic);
            //dt = ds.Tables[0];            
            dt = Session["dt_IT_Zoom"] as DataTable;

            DataTable dt1, dt2 = new DataTable();
            dt2 = Session["dt_IT_Tiempo"] as DataTable;

            object[,] arrData = new object[24, 7];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 12; i++)
            {
                arrData[i, 0] = dt2.Rows[i][1].ToString();
                //if (dt2.Rows[i][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            //int total = dt.Rows.Count;
            //int Smeses = total - 12;

            for (int i = 0; i < 12; i++)
            {
                arrData[i, 1] = dt.Rows[i][2].ToString();
                arrData[i, 2] = dt.Rows[i][3].ToString();
                arrData[i, 3] = dt.Rows[i][4].ToString();
                arrData[i, 4] = dt.Rows[i][5].ToString();
            }

            //dt1 = ds.Tables[1];
            dt1 = Session["dt_IT_Gridview"] as DataTable;

            GridViewIT.DataSource = tablaFormato_gv_Reporte_IT(dt1);
            GridViewIT.DataBind();


            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder();
            StringBuilder serieD = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/

            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/  
                    "'showValues': '0'," +
                //"'anchorRadius': '6'," +
                    "'lineThickness': '3'," +
                //"'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                    "'paletteColors': '#FF4F00,#548235,#2E75B6,#9DC3E6'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + sSerieNombre1 + "'");
            serieB.Append("{'seriesname': '" + sSerieNombre2 + "', 'alpha': '75' ");
            serieC.Append("{'seriesname': '" + sSerieNombre3 + "'");
            serieD.Append("{'seriesname': '" + sSerieNombre4 + "', 'alpha': '75' ");


            /*CUERPO*/
            for (int i = 0; i < 12; i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");
                    serieD.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");
                    serieD.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");
            serieC.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieD.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());
            jsonData.Append(serieD.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");


            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartIT_Zoom", "100%", "400px", "json", jsonData.ToString());
            LiteralIT.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, "myChartIT_Zoom_Print", "540px", "220px", "json", jsonData.ToString());
            LiteralIT_Print.Text = sales_print.Render();
        }

        /*************Graficas-Tercera FIla*************/
        protected void GraficaDistribucionIngreso(String sDisenio, int codclavecic)
        {
            ds = DistribucionIngresoBL.instancia.RCLI_OBT_DIST_IXS(codclavecic);
            dt = ds.Tables[1];

            Session["dt_IXS_Titulo"] = ds.Tables[0];
            Session["dt_IXS_Zoom"] = ds.Tables[1];
            Session["dt_IXS_Gridview"] = ds.Tables[2];
            Session["dt_IXS_Tiempo"] = ds.Tables[3];

            object[,] arrData = new object[12, 5];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 6; i++)
            {
                arrData[i, 0] = ds.Tables[3].Rows[i + 6][1].ToString();
                //if (ds.Tables[3].Rows[i + 6][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            int total = dt.Rows.Count;
            int Smeses = total - 6;

            for (int i = 0; i < 6; i++)
            {

                arrData[i, 1] = dt.Rows[i + Smeses][2].ToString(); //IXS1                
                arrData[i, 2] = dt.Rows[i + Smeses][3].ToString();//IXS2
                arrData[i, 3] = dt.Rows[i + Smeses][4].ToString();//IXS3
                arrData[i, 4] = dt.Rows[i + Smeses][5].ToString();//OTROS
            }

            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder();
            StringBuilder serieD = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/

            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/  
                    "'showValues': '0'," +
                    "'anchorRadius': '4'," +
                    "'lineThickness': '3'," +
                //"'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                    "'paletteColors': '#FFC000,#FF4F00,#2E75B6,#9DC3E6'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + ds.Tables[0].Rows[0][0] + "','anchorBgColor':'#FFC000'");
            serieB.Append("{'seriesname': '" + ds.Tables[0].Rows[1][0] + "','anchorBgColor':'#FF4F00'");
            serieC.Append("{'seriesname': '" + ds.Tables[0].Rows[2][0] + "','anchorBgColor':'#2E75B6'");
            serieD.Append("{'seriesname': 'Otros','anchorBgColor':'#9DC3E6'");

            /*CUERPO*/
            for (int i = 0; i < 6; i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");
                    serieD.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");
                    serieD.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");
            serieC.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieD.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());
            jsonData.Append(serieD.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartIXS", "100%", "85%", "json", jsonData.ToString());
            Panel_IXS.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, "myChartIXS_print", "350px", "200px", "json", jsonData.ToString());
            Panel_IXS_print.Text = sales_print.Render();
        }
        protected void GraficaDistribucionIngreso_Zoom(String sDisenio, int codclavecic)
        {
            //ds = DistribucionIngresoBL.instancia.RCLI_OBT_DIST_IXS(codclavecic);
            //dt = ds.Tables[1];

            dt = Session["dt_IXS_Zoom"] as DataTable;
            //Session["dt_IXS_Titulo"] = ds.Tables[0];

            DataTable dt1, dt2, dt3 = new DataTable();
            dt3 = Session["dt_IXS_Tiempo"] as DataTable;


            object[,] arrData = new object[24, 7];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 12; i++)
            {
                arrData[i, 0] = ds.Tables[3].Rows[i][1].ToString();
                //if (ds.Tables[3].Rows[i + 6][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            //int total = dt.Rows.Count;
            //int Smeses = total - 6;

            for (int i = 0; i < 12; i++)
            {

                arrData[i, 1] = dt.Rows[i][2].ToString(); //IXS1                
                arrData[i, 2] = dt.Rows[i][3].ToString();//IXS2
                arrData[i, 3] = dt.Rows[i][4].ToString();//IXS3
                arrData[i, 4] = dt.Rows[i][5].ToString();//OTROS
            }

            dt1 = Session["dt_IXS_Gridview"] as DataTable;

            GridViewIXS.DataSource = tablaFormato_gv_Reporte_IT(dt1);
            GridViewIXS.DataBind();

            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder();
            StringBuilder serieD = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/

            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/  
                    "'showValues': '0'," +
                    "'anchorRadius': '4'," +
                    "'lineThickness': '3'," +
                //"'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                    "'paletteColors': '#FFC000,#FF4F00,#2E75B6,#9DC3E6'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");

            categories.Append("'categories': [{'category': [");

            dt2 = Session["dt_IXS_Titulo"] as DataTable;

            serieA.Append("{'seriesname': '" + dt2.Rows[0][0] + "','anchorBgColor':'#FFC000'");
            serieB.Append("{'seriesname': '" + dt2.Rows[1][0] + "','anchorBgColor':'#FF4F00'");
            serieC.Append("{'seriesname': '" + dt2.Rows[2][0] + "','anchorBgColor':'#2E75B6'");
            serieD.Append("{'seriesname': 'Otros','anchorBgColor':'#9DC3E6'");

            /*CUERPO*/
            for (int i = 0; i < 12; i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");
                    serieD.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");
                    serieD.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");
            serieC.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieD.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());
            jsonData.Append(serieD.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartIXS_Zoom", "100%", "70%", "json", jsonData.ToString());
            LiteralIXS.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, "myChartIXS_Zoom_Print", "540px", "220px", "json", jsonData.ToString());
            LiteralIXS_Print.Text = sales_print.Render();
        }

        //Uso de productos
        protected void GraficaUsoProducto(int codclavecic)
        {
            dUsoProducto.Style.Add("display", "block");
            ds = UsoProductoBL.instancia.RCLI_OBT_MATRIZDEUSO(codclavecic);
            dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                Double real = 0;
                Double total = 0;
                Double porcentaje = 0;

                real = Convert.ToDouble(dt.Rows[0][3].ToString());
                total = Convert.ToDouble(dt.Rows[0][4].ToString());

                porcentaje = (real / total) * 100;
                barraMAFill.Attributes.Remove("data-fill");
                barraMAFill.Attributes.Add("data-fill", String.Format("{0:0}", porcentaje.ToString()));
                barraMAText.InnerText = real.ToString() + " de " + total.ToString();

                real = Convert.ToDouble(dt.Rows[1][3].ToString());
                total = Convert.ToDouble(dt.Rows[1][4].ToString());

                porcentaje = (real / total) * 100;
                barraMPFill.Attributes.Remove("data-fill");
                barraMPFill.Attributes.Add("data-fill", String.Format("{0:0}", porcentaje.ToString()));
                barraMPText.InnerText = real.ToString() + " de " + total.ToString();

                real = Convert.ToDouble(dt.Rows[2][3].ToString());
                total = Convert.ToDouble(dt.Rows[2][4].ToString());

                porcentaje = (real / total) * 100;
                barraComexFill.Attributes.Remove("data-fill");
                barraComexFill.Attributes.Add("data-fill", String.Format("{0:0}", porcentaje.ToString()));
                barraComexText.InnerText = real.ToString() + " de " + total.ToString();

                real = Convert.ToDouble(dt.Rows[3][3].ToString());
                total = Convert.ToDouble(dt.Rows[3][4].ToString());

                porcentaje = (real / total) * 100;
                barraTesoreriaFill.Attributes.Remove("data-fill");
                barraTesoreriaFill.Attributes.Add("data-fill", String.Format("{0:0}", porcentaje.ToString()));
                barraTesoreriaText.InnerText = real.ToString() + " de " + total.ToString();

                real = Convert.ToDouble(dt.Rows[4][3].ToString());
                total = Convert.ToDouble(dt.Rows[4][4].ToString());

                porcentaje = (real / total) * 100;
                barraOtrosFill.Attributes.Remove("data-fill");
                barraOtrosFill.Attributes.Add("data-fill", String.Format("{0:0}", porcentaje.ToString()));
                barraOtrosText.InnerText = real.ToString() + " de " + total.ToString();
            }

        }
        protected void GraficaUsoProducto_Zoom(int codclavecic)
        {

            ds = UsoProductoBL.instancia.RCLI_OBT_MATRIZDEUSO_ZOOM(codclavecic);
            dt = ds.Tables[0];

            tM1_COMEX.Rows.Clear();
            tM1_PRECOM.Rows.Clear();
            tM1_FINCOR.Rows.Clear();
            tM1_OTROS.Rows.Clear();
            tM2_CTACTE.Rows.Clear();
            tM2_RECPAG.Rows.Clear();
            tM2_OTROS.Rows.Clear();
            tM3_COMEX.Rows.Clear();
            tM4_TESO.Rows.Clear();
            tM5_COBR.Rows.Clear();
            tM5_CONT.Rows.Clear();
            tM5_GGTT.Rows.Clear();
            tM5_RECPAG.Rows.Clear();
            tM5_OTROS.Rows.Clear();


            //tTODOS.Rows.Clear();



            tUsoProductos_1.Rows.Clear();
            tUsoProductos_2.Rows.Clear();
            tUsoProductos_3.Rows.Clear();
            tUsoProductos_4.Rows.Clear();
            tUsoProductos_5.Rows.Clear();






            Cargartabla(tM1_COMEX, "M1", "COMEX", dt);
            Cargartabla(tM1_PRECOM, "M1", "PRECOM", dt);
            Cargartabla(tM1_FINCOR, "M1", "FINCOR", dt);
            Cargartabla(tM1_OTROS, "M1", "OTROS", dt);
            Cargartabla(tM2_CTACTE, "M2", "CTACTE", dt);
            Cargartabla(tM2_RECPAG, "M2", "RECPAG", dt);
            Cargartabla(tM2_OTROS, "M2", "OTROS", dt);
            Cargartabla(tM3_COMEX, "M3", "COMEX", dt);
            Cargartabla(tM4_TESO, "M4", "TESO", dt);
            Cargartabla(tM5_COBR, "M5", "COBR", dt);
            Cargartabla(tM5_CONT, "M5", "CONT", dt);
            Cargartabla(tM5_GGTT, "M5", "GGTT", dt);
            Cargartabla(tM5_RECPAG, "M5", "RECPAG", dt);
            Cargartabla(tM5_OTROS, "M5", "OTROS", dt);

            //Cargartabla(tTODOS, "TODOS", "TODOS", dt);




            Cargartabla(tUsoProductos_1, "M1", "TODOS", dt);
            Cargartabla(tUsoProductos_2, "M2", "TODOS", dt);
            Cargartabla(tUsoProductos_3, "M3", "TODOS", dt);
            Cargartabla(tUsoProductos_4, "M4", "TODOS", dt);
            Cargartabla(tUsoProductos_5, "M5", "TODOS", dt);





        }
        protected void Cargartabla(Table tTabla, String sFila, String sCodFamilia, DataTable dTabla)
        {
            DataTable dTablaFilter = new DataTable();

            int iAltoTexto = 50;
            int iAnchoTexto = 177;
            int iAltoAnchoImagen = 25;
            string simgKpiCheck = "~/img/icon_check.png";
            string simgKpiCross = "~/img/icon_cross.png";

            if (sFila == "TODOS" && sCodFamilia == "TODOS")
            {
                dTablaFilter = dTabla.Select("", "DESCPRODUCTO, REAL DESC").CopyToDataTable();

                iAnchoTexto = 140;
                iAltoAnchoImagen = 12;
                simgKpiCross = "~/img/icon_cross_light.png";
            }
            else
            {
                if (sCodFamilia == "TODOS" && (sFila == "M1" || sFila == "M2" || sFila == "M3" || sFila == "M4" || sFila == "M5"))
                {
                    dTablaFilter = dTabla.Select("FILA = '" + sFila + "'", "REAL DESC").CopyToDataTable();
                    iAnchoTexto = 140;
                    iAltoAnchoImagen = 12;
                    simgKpiCross = "~/img/icon_cross_light.png";
                }
                else
                {
                    dTablaFilter = dTabla.Select("FILA = '" + sFila + "' AND CODFAMILIA = '" + sCodFamilia + "'", "REAL DESC").CopyToDataTable();
                }
            }


            Image imgKpi = new Image();
            TableRow tRow = new TableRow();
            TableCell tCell = new TableCell(); ;
            int iCont = 1;
            foreach (DataRow dtr in dTablaFilter.Rows)
            {
                //-----------------------------
                tCell = new TableCell();
                tCell.Height = iAltoTexto;
                tCell.Text = dtr["DESCPRODUCTO"].ToString();
                tCell.Width = iAnchoTexto;
                tRow.Cells.Add(tCell);
                //-----------------------------
                tCell = new TableCell();
                tCell.Height = iAltoTexto;
                if (dtr["REAL"].ToString() == "1")
                {
                    imgKpi = new Image();
                    imgKpi.ImageUrl = simgKpiCheck;
                    imgKpi.Height = iAltoAnchoImagen;
                    imgKpi.Width = iAltoAnchoImagen;
                    tCell.Controls.Add(imgKpi);
                }
                else
                {
                    imgKpi = new Image();
                    imgKpi.ImageUrl = simgKpiCross;
                    imgKpi.Height = iAltoAnchoImagen;
                    imgKpi.Width = iAltoAnchoImagen;
                    tCell.Controls.Add(imgKpi);
                }
                tRow.Cells.Add(tCell);

                if (iCont % 4 == 0)
                {
                    tTabla.Rows.Add(tRow);
                    tRow = new TableRow();
                }
                iCont++;
            }
            tTabla.Rows.Add(tRow);
        }

        //Dist. PDM
        protected void GraficaDistribucionPdm(String sDisenio, String sSerieNombre1, String sSerieNombre2, int codclavecic)
        {
            ds = DistribucionPdmBL.instancia.RCLI_OBT_DIST_PDM(codclavecic);
            if (ds.Tables.Count == 3)
            {
                dt = ds.Tables[0];

                Session["dt_DPDM_Zoom"] = ds.Tables[0];
                Session["dt_DPDM_Titulo"] = ds.Tables[1];
                Session["dt_DPDM_Gridview"] = ds.Tables[2];


                object[,] arrData = new object[8, 3];

                //Store product labels in the first column.
                String currentYear = DateTime.Now.Year.ToString();
                //int cont = 0;

                for (int i = 0; i < 7; i++)
                {
                    arrData[i, 0] = ds.Tables[0].Rows[i][1].ToString();
                    //if (ds.Tables[3].Rows[i + 6][0].ToString() != currentYear)
                    //{
                    //    cont = cont + 1;
                    //}
                }

                //Store sales data for the current year in the second column.
                int total = dt.Rows.Count;
                int Smeses = total - 6;

                for (int i = 0; i < 7; i++)
                {

                    arrData[i, 1] = dt.Rows[i][2].ToString(); //IXS1                
                    arrData[i, 2] = dt.Rows[i][3].ToString();//IXS2
                    //arrData[i, 3] = dt.Rows[i + Smeses][4].ToString();//IXS3
                    //arrData[i, 4] = dt.Rows[i + Smeses][5].ToString();//OTROS
                }

                StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
                StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

                StringBuilder serieA = new StringBuilder();
                StringBuilder serieB = new StringBuilder();
                //StringBuilder serieC = new StringBuilder();
                //StringBuilder serieD = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/

                /*CABEZA*/
                jsonData.Append("{" +
               "'chart': {" +
                    //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                    //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/  
                        "'showValues': '0'," +
                        "'anchorRadius': '0'," +
                        "'lineThickness': '3'," +
                    //"'anchorBgColor': '#FFC000'," +
                        "'sNumberSuffix': '%'," +
                    //"'paletteColors': '#2E75B6,#FF4F00" +
                        "'baseFontColor': '#7F7F7F'," +
                    /*CAJA EXTERNA DE LA GRAFIA*/
                        "'showBorder': '0', " +
                        "'bgColor': '#ffffff'," +
                        "'radarfillcolor': '#ffffff'," +
                        "'showShadow': '0'," +
                    /*CAJA INTERNA DE LA GRAFICA*/
                        "'canvasBgColor': '#ffffff'," +
                    /*LINEA DIVISORIA DE CAJA INTERNA*/
                        "'canvasBorderAlpha': '0'," +
                    /*GRAFICAS*/
                        "'usePlotGradientColor': '0'," +
                        "'showplotborder': '0'," +
                        "'showXAxisLine': '1'," +
                        "'xAxisLineColor': '#999999'," +
                        "'showAlternateHGridColor': '0'," +
                        "'showAlternateVGridColor': '0'," +
                        "'legendBgAlpha': '0'," +
                        "'legendBorderAlpha': '0'," +
                        "'legendItemFontSize': '8'," +
                        "'formatNumberScale': '0'," +
                        "'placeValuesInside': '1'," +
                        "'plotFillAlpha': '90'," +
                        "'decimals': '2'" +
                    "},");



                categories.Append("'categories': [{'category': [");

                serieA.Append("{'seriesname': '" + sSerieNombre1 + "','color':'#FF4F00','anchorbgcolor':'#FF4F00'");
                serieB.Append("{'seriesname': '" + sSerieNombre2 + "','color':'#2E75B6','anchorbgcolor':'#2E75B6', 'alpha': '50' ");
                //serieC.Append("{'seriesname': '" + ds.Tables[0].Rows[2][0] + "','anchorBgColor':'#2E75B6'");
                //serieD.Append("{'seriesname': 'Otros','anchorBgColor':'#9DC3E6'");

                /*CUERPO*/
                for (int i = 0; i < 7; i++)
                {
                    if (i == 0)
                    {
                        serieA.Append(",'data': [");
                        serieB.Append(",'data': [");
                        //serieC.Append(",'data': [");
                        //serieD.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                    }

                    if (i > 0)
                    {
                        categories.Append(",");
                        serieA.Append(",");
                        serieB.Append(",");
                        //serieC.Append(",");
                        //serieD.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                    }

                    categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                    serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                    serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                    //serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                    //serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }



                /*PIE*/
                categories.Append("]}],");
                serieA.Append("]},");
                serieB.Append("]},");
                //serieC.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
                //serieD.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


                /*CIERRE*/
                jsonData.Append(categories.ToString());
                jsonData.Append("'dataset': [");
                jsonData.Append(serieA.ToString());
                jsonData.Append(serieB.ToString());
                //jsonData.Append(serieC.ToString());
                //jsonData.Append(serieD.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                jsonData.Append("]" + "}");



                /*RENDERIZAR*/
                Chart sales = new Chart(sDisenio, "myChartDPDM", "100%", "88%", "json", jsonData.ToString());
                Panel_DPDM.Text = sales.Render();

                Chart sales_print = new Chart(sDisenio, "myChartDPDM_print", "350px", "215px", "json", jsonData.ToString());
                Panel_DPDM_print.Text = sales_print.Render();
            }
        }
        protected void GraficaDistribucionPdm_Zoom(String sDisenio, String sSerieNombre1, String sSerieNombre2, int codclavecic)
        {
            //ds = DistribucionPdmBL.instancia.RCLI_OBT_DIST_PDM(codclavecic);
            //dt = ds.Tables[0];

            dt = Session["dt_DPDM_Zoom"] as DataTable;

            object[,] arrData = new object[8, 3];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 7; i++)
            {
                arrData[i, 0] = dt.Rows[i][1].ToString();
                //if (ds.Tables[3].Rows[i + 6][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            int total = dt.Rows.Count;
            int Smeses = total - 6;

            for (int i = 0; i < 7; i++)
            {

                arrData[i, 1] = dt.Rows[i][2].ToString(); //IXS1                
                arrData[i, 2] = dt.Rows[i][3].ToString();//IXS2
                //arrData[i, 3] = dt.Rows[i + Smeses][4].ToString();//IXS3
                //arrData[i, 4] = dt.Rows[i + Smeses][5].ToString();//OTROS
            }

            DataTable dt1 = new DataTable();
            dt1 = Session["dt_DPDM_Gridview"] as DataTable;

            GridViewDPDM.DataSource = tablaFormato_gv_Reporte_DPDM(dt1);
            GridViewDPDM.DataBind();


            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            //StringBuilder serieC = new StringBuilder();
            //StringBuilder serieD = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/

            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/  
                    "'showValues': '0'," +
                    "'anchorRadius': '0'," +
                    "'lineThickness': '3'," +
                //"'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                //"'paletteColors': '#2E75B6,#FF4F00" +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'radarfillcolor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    "'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");



            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + sSerieNombre1 + "','color':'#FF4F00','anchorbgcolor':'#FF4F00'");
            serieB.Append("{'seriesname': '" + sSerieNombre2 + "','color':'#2E75B6','anchorbgcolor':'#2E75B6', 'alpha': '50' ");
            //serieC.Append("{'seriesname': '" + ds.Tables[0].Rows[2][0] + "','anchorBgColor':'#2E75B6'");
            //serieD.Append("{'seriesname': 'Otros','anchorBgColor':'#9DC3E6'");

            /*CUERPO*/
            for (int i = 0; i < 7; i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    //serieC.Append(",'data': [");
                    //serieD.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    //serieC.Append(",");
                    //serieD.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                //serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                //serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");
            //serieC.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            //serieD.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            //jsonData.Append(serieC.ToString());
            //jsonData.Append(serieD.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartDPDM_Zoom", "100%", "75%", "json", jsonData.ToString());
            LiteralDPDM.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, "myChartDPDM_Zoom_Print", "500px", "500px", "json", jsonData.ToString());
            LiteralDPDM_Print.Text = sales_print.Render();
        }

        //Trend PDM
        protected void GraficaTrenPdm(String sDisenio, int codclavecic)
        {
            ds = TrendPdmBL.instancia.RCLI_OBT_PDM(codclavecic);
            dt = ds.Tables[1];

            Session["dt_PDM_Titulo"] = ds.Tables[0];
            Session["dt_PDM_Zoom"] = ds.Tables[1];
            Session["dt_PDM_Gridview"] = ds.Tables[2];
            Session["dt_PDM_Tiempo"] = ds.Tables[3];

            object[,] arrData = new object[6, 6];

            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 6; i++)
            {
                arrData[i, 0] = ds.Tables[3].Rows[i + 6][1].ToString();
                //if (ds.Tables[3].Rows[i + 6][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            int total = dt.Rows.Count;
            int Smeses = total - 6;

            //Store sales data for the current year in the second column.
            for (int i = 0; i < 6; i++)
            {
                arrData[i, 1] = dt.Rows[i + Smeses][2].ToString();
                arrData[i, 2] = dt.Rows[i + Smeses][3].ToString();
                arrData[i, 3] = dt.Rows[i + Smeses][4].ToString();
                arrData[i, 4] = dt.Rows[i + Smeses][5].ToString();
                arrData[i, 5] = dt.Rows[i + Smeses][6].ToString();
            }

            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            StringBuilder serieD = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            StringBuilder serieE = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/
                    "'showValues': '0'," +
                    "'anchorRadius': '4'," +
                    "'lineThickness': '3'," +
                //"'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                //"'paletteColors': '#FF4F00,#2E75B6,#FFC000'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");


            DataTable dt1 = new DataTable();
            dt1 = ds.Tables[0];

            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + dt1.Rows[0][0] + "', 'color':'#D8D8D8'");
            serieB.Append("{'seriesname': '" + dt1.Rows[0][1] + "', 'parentYAxis': 'S','color':'" + dt1.Rows[1][1] + "','anchorBgColor':'" + dt1.Rows[1][1] + "','renderAs': 'line'");
            serieC.Append("{'seriesname': '" + dt1.Rows[0][2] + "', 'parentYAxis': 'S','color':'" + dt1.Rows[1][2] + "','anchorBgColor':'" + dt1.Rows[1][2] + "','renderAs': 'line'");
            serieD.Append("{'seriesname': '" + dt1.Rows[0][3] + "', 'parentYAxis': 'S','color':'" + dt1.Rows[1][3] + "','anchorBgColor':'" + dt1.Rows[1][3] + "','renderAs': 'line'");
            serieE.Append("{'seriesname': '" + dt1.Rows[0][4] + "', 'parentYAxis': 'S','color':'#FFC000','anchorBgColor':'#FFC000','renderAs': 'line'");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CUERPO*/
            for (int i = 0; i < arrData.GetLength(0); i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");
                    serieD.Append(",'data': [");
                    serieE.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/

                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");
                    serieD.Append(",");
                    serieE.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);
                serieE.AppendFormat("{{'value': '{0}'}}", arrData[i, 5]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");
            serieC.Append("]},");
            serieD.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieE.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());
            jsonData.Append(serieD.ToString());
            jsonData.Append(serieE.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartTPDM", "100%", "85%", "json", jsonData.ToString());
            Panel_TPDM.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, "myChartTPDM_print", "350px", "215px", "json", jsonData.ToString());
            Panel_TPDM_print.Text = sales_print.Render();
        }

        protected void GraficaTrenPdm_Zoom(String sDisenio, int codclavecic)
        {
            //ds = TrendPdmBL.instancia.RCLI_OBT_PDM(codclavecic);
            //dt = ds.Tables[1];
            dt = Session["dt_PDM_Zoom"] as DataTable;

            DataTable dt2, dt3 = new DataTable();
            dt3 = Session["dt_PDM_Tiempo"] as DataTable;

            object[,] arrData = new object[12, 7];
            //Store product labels in the first column.
            String currentYear = DateTime.Now.Year.ToString();
            //int cont = 0;

            for (int i = 0; i < 12; i++)
            {
                arrData[i, 0] = dt3.Rows[i][1].ToString();
                //if (ds.Tables[3].Rows[i + 6][0].ToString() != currentYear)
                //{
                //    cont = cont + 1;
                //}
            }

            //Store sales data for the current year in the second column.
            for (int i = 0; i < 12; i++)
            {
                arrData[i, 1] = dt.Rows[i][2].ToString();
                arrData[i, 2] = dt.Rows[i][3].ToString();
                arrData[i, 3] = dt.Rows[i][4].ToString();
                arrData[i, 4] = dt.Rows[i][5].ToString();
                arrData[i, 5] = dt.Rows[i][6].ToString();
            }

            dt2 = Session["dt_PDM_Gridview"] as DataTable;

            GridViewPDM.DataSource = tablaFormato_gv_Reporte_PDM(dt2);
            GridViewPDM.DataBind();

            StringBuilder jsonData = new StringBuilder();//Caracteristicas y estilos
            StringBuilder categories = new StringBuilder();//Carteles, Cabeceras o Etiquetas

            StringBuilder serieA = new StringBuilder();
            StringBuilder serieB = new StringBuilder();
            StringBuilder serieC = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            StringBuilder serieD = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            StringBuilder serieE = new StringBuilder(); /*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CABEZA*/
            jsonData.Append("{" +
           "'chart': {" +
                //"'caption': 'Sales by Product'," +            /*SE RETIRA EL ENCABEZADO*/
                //"'numberPrefix': 'S/'," +                     /*SE RETIRA EL PREFIJO DE SOLES*/
                    "'showValues': '0'," +
                    "'anchorRadius': '4'," +
                    "'lineThickness': '3'," +
                //"'anchorBgColor': '#FFC000'," +
                    "'sNumberSuffix': '%'," +
                //"'paletteColors': '#FF4F00,#2E75B6,#FFC000'," +
                    "'baseFontColor': '#7F7F7F'," +
                /*CAJA EXTERNA DE LA GRAFIA*/
                    "'showBorder': '0', " +
                    "'bgColor': '#ffffff'," +
                    "'showShadow': '0'," +
                /*CAJA INTERNA DE LA GRAFICA*/
                    "'canvasBgColor': '#ffffff'," +
                /*LINEA DIVISORIA DE CAJA INTERNA*/
                    "'canvasBorderAlpha': '0'," +
                /*GRAFICAS*/
                    "'usePlotGradientColor': '0'," +
                    "'showplotborder': '0'," +
                    " 'showXAxisLine': '1'," +
                    "'xAxisLineColor': '#999999'," +
                    "'showAlternateHGridColor': '0'," +
                    "'showAlternateVGridColor': '0'," +
                    "'legendBgAlpha': '0'," +
                    "'legendBorderAlpha': '0'," +
                    "'legendItemFontSize': '8'," +
                    "'formatNumberScale': '0'," +
                    "'placeValuesInside': '1'," +
                    "'plotFillAlpha': '90'," +
                    "'decimals': '2'" +
                "},");


            DataTable dt1 = new DataTable();
            dt1 = ds.Tables[0];

            categories.Append("'categories': [{'category': [");

            serieA.Append("{'seriesname': '" + dt1.Rows[0][0] + "', 'color':'#D8D8D8'");
            serieB.Append("{'seriesname': '" + dt1.Rows[0][1] + "', 'parentYAxis': 'S','color':'" + dt1.Rows[1][1] + "','anchorBgColor':'" + dt1.Rows[1][1] + "','renderAs': 'line'");
            serieC.Append("{'seriesname': '" + dt1.Rows[0][2] + "', 'parentYAxis': 'S','color':'" + dt1.Rows[1][2] + "','anchorBgColor':'" + dt1.Rows[1][2] + "','renderAs': 'line'");
            serieD.Append("{'seriesname': '" + dt1.Rows[0][3] + "', 'parentYAxis': 'S','color':'" + dt1.Rows[1][3] + "','anchorBgColor':'" + dt1.Rows[1][3] + "','renderAs': 'line'");
            serieE.Append("{'seriesname': '" + dt1.Rows[0][4] + "', 'parentYAxis': 'S','color':'#FFC000','anchorBgColor':'#FFC000','renderAs': 'line'");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CUERPO*/
            for (int i = 0; i < arrData.GetLength(0); i++)
            {
                if (i == 0)
                {
                    serieA.Append(",'data': [");
                    serieB.Append(",'data': [");
                    serieC.Append(",'data': [");
                    serieD.Append(",'data': [");
                    serieE.Append(",'data': [");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/

                }

                if (i > 0)
                {
                    categories.Append(",");
                    serieA.Append(",");
                    serieB.Append(",");
                    serieC.Append(",");
                    serieD.Append(",");
                    serieE.Append(",");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
                }

                categories.AppendFormat("{{'label': '{0}'}}", arrData[i, 0]);
                serieA.AppendFormat("{{'value': '{0}'}}", arrData[i, 1]);
                serieB.AppendFormat("{{'value': '{0}'}}", arrData[i, 2]);
                serieC.AppendFormat("{{'value': '{0}'}}", arrData[i, 3]);
                serieD.AppendFormat("{{'value': '{0}'}}", arrData[i, 4]);
                serieE.AppendFormat("{{'value': '{0}'}}", arrData[i, 5]);/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            }



            /*PIE*/
            categories.Append("]}],");
            serieA.Append("]},");
            serieB.Append("]},");
            serieC.Append("]},");
            serieD.Append("]},");/*No olvidar que sigue otro grafico mas :D*/
            serieE.Append("]}");/*CAMBIO INGRESO DE UNA NUEVA SERIE*/


            /*CIERRE*/
            jsonData.Append(categories.ToString());
            jsonData.Append("'dataset': [");
            jsonData.Append(serieA.ToString());
            jsonData.Append(serieB.ToString());
            jsonData.Append(serieC.ToString());
            jsonData.Append(serieD.ToString());
            jsonData.Append(serieE.ToString());/*CAMBIO INGRESO DE UNA NUEVA SERIE*/
            jsonData.Append("]" + "}");



            /*RENDERIZAR*/
            Chart sales = new Chart(sDisenio, "myChartTPDM_Zoom", "100%", "70%", "json", jsonData.ToString());
            LiteralTPDM.Text = sales.Render();

            Chart sales_print = new Chart(sDisenio, "myChartTPDM_Zoom_Print", "540px", "250px", "json", jsonData.ToString());
            LiteralTPDM_Print.Text = sales_print.Render();
        }

        /*******Gridview*************/
        //Evento de stilos del grafico Indicador File
        protected void gvData1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridView HeaderGrid = (GridView)sender;
                GridViewRow HeaderGridRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                TableCell HeaderCell = new TableCell();


                //-----------------------------------------------------------------------------

                HeaderCell.Text = " ";
                HeaderCell.ColumnSpan = 2;
                HeaderCell.CssClass = "sCabecera_principal sCol15912";

                HeaderGridRow.Cells.Add(HeaderCell);
                HeaderCell.Font.Size = FontUnit.Small;

                //-----------------------------------------------------------------------------

                HeaderCell = new TableCell();
                HeaderCell.Text = "Año Actual (M S/)";
                HeaderCell.ColumnSpan = 4;
                HeaderCell.CssClass = "sCabecera_principal sCol15912";


                HeaderGridRow.Cells.Add(HeaderCell);
                HeaderCell.Font.Size = FontUnit.Small;

                //-----------------------------------------------------------------------------

                HeaderCell = new TableCell();
                HeaderCell.Text = "Año Anterior (M S/)";
                HeaderCell.ColumnSpan = 4;
                HeaderCell.CssClass = "sCabecera_principal sCol15912";


                HeaderGridRow.Cells.Add(HeaderCell);
                HeaderCell.Font.Size = FontUnit.Small;

                //-----------------------------------------------------------------------------

                HeaderCell = new TableCell();
                HeaderCell.Text = "Saldos (M S/)";
                HeaderCell.ColumnSpan = 3;
                HeaderCell.CssClass = "sCabecera_principal sCol15912";


                HeaderGridRow.Cells.Add(HeaderCell);
                HeaderCell.Font.Size = FontUnit.Small;

                //-----------------------------------------------------------------------------

                HeaderGridRow.BorderColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                gvData1.Controls[0].Controls.AddAt(0, HeaderGridRow);
                gvData1.HeaderStyle.Font.Bold = true;


            }
        }

        /*******Formato*************/
        //Función para el formato de número sin decimales
        public string FormatoNumDecimal(String valor, int cantDecimal, int porcentaje)
        {
            String valorFormato = "";

            if (!String.IsNullOrEmpty(valor))
            {
                switch (cantDecimal)
                {
                    case 0:
                        valorFormato = Math.Round(validarDouble(valor) * porcentaje).ToString("###,###,###0.");
                        break;
                    case 1:
                        valorFormato = Math.Round(validarDouble(valor) * porcentaje, 1, MidpointRounding.AwayFromZero).ToString("###,###,###0.0");
                        break;
                    case 2:
                        valorFormato = Math.Round(validarDouble(valor) * porcentaje, 2, MidpointRounding.AwayFromZero).ToString("###,###,###0.00");
                        break;
                    case 3:
                        valorFormato = Math.Round(validarDouble(valor) * porcentaje, 3, MidpointRounding.AwayFromZero).ToString("###,###,###0.000");
                        break;
                    case 4:
                        valorFormato = Math.Round(validarDouble(valor) * porcentaje, 4, MidpointRounding.AwayFromZero).ToString("###,###,###0.0000");
                        break;
                }
                if (porcentaje == 100)
                {
                    valorFormato = valorFormato + "%";
                }
            }
            return valorFormato;
        }
        //Validar Double
        public Double validarDouble(String valor)
        {
            Double valorValidado = 0;

            if (Double.TryParse(valor, out valorValidado))
            {
                valorValidado = Convert.ToDouble(valor);
                if (Double.IsInfinity(valorValidado) || Double.IsNaN(valorValidado))
                {
                    valorValidado = 0;
                }
            }
            return valorValidado;
        }
        //Formato Gridview
        public DataTable tablaFormato_gv_Reporte(DataTable dt)
        {
            dtCloned = null;
            dtCloned = dt.Clone();

            for (int j = 0; j <= dtCloned.Columns.Count - 1; j++)
            {
                if (j >= 0)
                {
                    dtCloned.Columns[j].DataType = typeof(String);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                dtCloned.ImportRow(row);
            }

            DataTable dt1 = new DataTable();
            dt1 = Session["dt_Colocacion_Tiempo"] as DataTable;

            //Cabecera
            for (int j = 0; j <= dtCloned.Columns.Count - 1; j++)
            {
                if (j == 0)
                {
                    dtCloned.Columns[j].ColumnName = " ";
                }
                else
                {
                    dtCloned.Columns[j].ColumnName = dt1.Rows[j - 1][2].ToString();
                }
            }

            //Formato
            for (int i = 0; i <= dtCloned.Rows.Count - 1; i++)
            {
                for (int j = 1; j <= dtCloned.Columns.Count - 1; j++)
                {
                    switch (i)
                    {
                        case 0:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 100);
                            break;
                        case 1:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 1);
                            break;
                    }
                }
            }

            return dtCloned;
        }
        public DataTable tablaFormato_gv_Reporte_PDM(DataTable dt)
        {
            dtCloned = null;
            dtCloned = dt.Clone();

            for (int j = 0; j <= dtCloned.Columns.Count - 1; j++)
            {
                if (j >= 0)
                {
                    dtCloned.Columns[j].DataType = typeof(String);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                dtCloned.ImportRow(row);
            }

            DataTable dt1 = new DataTable();
            dt1 = Session["dt_PDM_Tiempo"] as DataTable;

            //Cabecera
            for (int j = 0; j <= dtCloned.Columns.Count - 1; j++)
            {
                if (j == 0)
                {
                    dtCloned.Columns[j].ColumnName = " ";
                }
                else
                {
                    dtCloned.Columns[j].ColumnName = dt1.Rows[j - 1][2].ToString();
                }
            }


            //Formato
            for (int i = 0; i <= dtCloned.Rows.Count - 1; i++)
            {
                for (int j = 1; j <= dtCloned.Columns.Count - 1; j++)
                {
                    switch (i)
                    {
                        case 0:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 1);
                            break;
                        case 1:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 100);
                            break;
                        case 2:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 100);
                            break;
                        case 3:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 100);
                            break;
                        case 4:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 100);
                            break;
                    }
                }
            }

            return dtCloned;
        }
        public DataTable tablaFormato_gv_Reporte_IT(DataTable dt)
        {
            dtCloned = null;
            dtCloned = dt.Clone();

            for (int j = 0; j <= dtCloned.Columns.Count - 1; j++)
            {
                if (j >= 0)
                {
                    dtCloned.Columns[j].DataType = typeof(String);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                dtCloned.ImportRow(row);
            }

            DataTable dt1 = new DataTable();
            dt1 = Session["dt_IT_Tiempo"] as DataTable;

            //Cabecera
            for (int j = 0; j <= dtCloned.Columns.Count - 1; j++)
            {
                if (j == 0)
                {
                    dtCloned.Columns[j].ColumnName = " ";
                }
                else
                {
                    dtCloned.Columns[j].ColumnName = dt1.Rows[j - 1][2].ToString();
                }
            }


            //Formato
            for (int i = 0; i <= dtCloned.Rows.Count - 1; i++)
            {
                for (int j = 1; j <= dtCloned.Columns.Count - 1; j++)
                {
                    switch (i)
                    {
                        case 0:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 1);
                            break;
                        case 1:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 1);
                            break;
                        case 2:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 1);
                            break;
                        case 3:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 1);
                            break;

                    }
                }
            }

            return dtCloned;
        }
        public DataTable tablaFormato_gv_Reporte_DPDM(DataTable dt)
        {
            dtCloned = null;
            dtCloned = dt.Clone();

            for (int j = 0; j <= dtCloned.Columns.Count - 1; j++)
            {
                if (j >= 0)
                {
                    dtCloned.Columns[j].DataType = typeof(String);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                dtCloned.ImportRow(row);
            }

            DataTable dt1 = new DataTable();
            dt1 = Session["dt_DPDM_Titulo"] as DataTable;

            //Cabecera
            for (int j = 0; j <= dt1.Rows.Count; j++)
            {
                if (j == 0)
                {
                    dtCloned.Columns[j].ColumnName = " ";
                }
                else
                {
                    dtCloned.Columns[j].ColumnName = dt1.Rows[j - 1][0].ToString();
                }
            }


            //Formato
            for (int i = 0; i <= dtCloned.Rows.Count - 1; i++)
            {
                for (int j = 1; j <= dtCloned.Columns.Count - 1; j++)
                {
                    switch (i)
                    {
                        case 0:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 1);
                            break;
                        case 1:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 1);
                            break;
                        case 2:
                            dtCloned.Rows[i][j] = FormatoNumDecimal(dt.Rows[i][j].ToString(), 2, 100);
                            break;
                    }
                }
            }

            return dtCloned;
        }
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {


            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].CssClass = "sCabecera_principal";




            }
        }

        protected void gv_RowDataBoundRARORAC(object sender, GridViewRowEventArgs e)
        {

            DataTable tb = new DataTable();
            tb = Session["dt_tiempo_TrendRarorac"] as DataTable;


            if (e.Row.RowType == DataControlRowType.Header)
            {

                e.Row.Cells[1].Text = tb.Rows[0][1].ToString();
                e.Row.Cells[2].Text = tb.Rows[1][1].ToString();
                e.Row.Cells[3].Text = tb.Rows[2][1].ToString();
                e.Row.Cells[4].Text = tb.Rows[3][1].ToString();
                e.Row.Cells[5].Text = tb.Rows[4][1].ToString();
                e.Row.Cells[6].Text = tb.Rows[5][1].ToString();
            }

            if (e.Row.RowIndex == 0)
            {
                e.Row.Cells[1].Text = String.Format("{0:0.0}%", Convert.ToDouble(e.Row.Cells[1].Text.ToString()) * 100);
                e.Row.Cells[2].Text = String.Format("{0:0.0}%", Convert.ToDouble(e.Row.Cells[2].Text.ToString()) * 100);
                e.Row.Cells[3].Text = String.Format("{0:0.0}%", Convert.ToDouble(e.Row.Cells[3].Text.ToString()) * 100);
                e.Row.Cells[4].Text = String.Format("{0:0.0}%", Convert.ToDouble(e.Row.Cells[4].Text.ToString()) * 100);
                e.Row.Cells[5].Text = String.Format("{0:0.0}%", Convert.ToDouble(e.Row.Cells[5].Text.ToString()) * 100);
                e.Row.Cells[6].Text = String.Format("{0:0.0}%", Convert.ToDouble(e.Row.Cells[6].Text.ToString()) * 100);
            }
            else if (e.Row.RowIndex >= 2 && e.Row.RowIndex <= 6)
            {
                e.Row.Cells[1].Text = String.Format("{0:,0.0}", Convert.ToDouble(e.Row.Cells[1].Text.ToString()) * 100);
                e.Row.Cells[2].Text = String.Format("{0:,0.0}", Convert.ToDouble(e.Row.Cells[2].Text.ToString()) * 100);
                e.Row.Cells[3].Text = String.Format("{0:,0.0}", Convert.ToDouble(e.Row.Cells[3].Text.ToString()) * 100);
                e.Row.Cells[4].Text = String.Format("{0:,0.0}", Convert.ToDouble(e.Row.Cells[4].Text.ToString()) * 100);
                e.Row.Cells[5].Text = String.Format("{0:,0.0}", Convert.ToDouble(e.Row.Cells[5].Text.ToString()) * 100);
                e.Row.Cells[6].Text = String.Format("{0:,0.0}", Convert.ToDouble(e.Row.Cells[6].Text.ToString()) * 100);
            }
            else if (e.Row.RowIndex == 1)
            {
                String PI = "";

                for (int i = 1; i < 7; i++)
                {

                    PI = e.Row.Cells[i].Text.ToString();

                    switch (PI)
                    {
                        case "0":
                            e.Row.Cells[i].Text = "-";
                            break;
                        case "0.0007":
                            e.Row.Cells[i].Text = "AAA+";
                            break;
                        case "0.0017":
                            e.Row.Cells[i].Text = "AAA";
                            break;
                        case "0.0031":
                            e.Row.Cells[i].Text = "AAA-";
                            break;
                        case "0.0054":
                            e.Row.Cells[i].Text = "AA";
                            break;
                        case "0.0087":
                            e.Row.Cells[i].Text = "A";
                            break;
                        case "0.0129":
                            e.Row.Cells[i].Text = "BBB+";
                            break;
                        case "0.0173":
                            e.Row.Cells[i].Text = "BBB";
                            break;
                        case "0.0218":
                            e.Row.Cells[i].Text = "BB+";
                            break;
                        case "0.0296":
                            e.Row.Cells[i].Text = "BB";
                            break;
                        case "0.042":
                            e.Row.Cells[i].Text = "B+";
                            break;
                        case "0.0579":
                            e.Row.Cells[i].Text = "B";
                            break;
                        case "0.0884":
                            e.Row.Cells[i].Text = "CCC";
                            break;
                        case "0.1271":
                            e.Row.Cells[i].Text = "CC";
                            break;
                        case "0.2156":
                            e.Row.Cells[i].Text = "C";
                            break;
                        case "0.3243":
                            e.Row.Cells[i].Text = "DDD";
                            break;
                        case "0.399":
                            e.Row.Cells[i].Text = "DD";
                            break;
                        case "0.4178":
                            e.Row.Cells[i].Text = "D";
                            break;
                        case "0.6786":
                            e.Row.Cells[i].Text = "EEE";
                            break;
                        case "0.7863":
                            e.Row.Cells[i].Text = "EE";
                            break;
                        default:
                            e.Row.Cells[i].Text = "-";
                            break;
                    }
                }
            }


        }
        /*******Formato*************/

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod()]
        public static void SimularRARORAC(String numero)
        {
            RARORACBL simularBL = new RARORACBL();

            Double delta = 0;
            delta = Convert.ToDouble(numero);
            delta = delta / 100;

            DataTable dt = null;

            dt = System.Web.HttpContext.Current.Session["dt_Cliente"] as DataTable;

            DataView dvStatic = new DataView();

            String idCliente = System.Web.HttpContext.Current.Session["IDCliente"].ToString();

            if (!String.IsNullOrEmpty(idCliente))
            {
                dvStatic = dt.DefaultView;
                dvStatic.RowFilter = "[CLIENTE] LIKE '" + idCliente + "' OR [IDC] LIKE '%" + idCliente + "%'";
            }

            dt = dvStatic.ToTable();

            ListaDatoClienteBL raroracCartera = new ListaDatoClienteBL();
            DatoCartera objCartera = new DatoCartera();
            objCartera = raroracCartera.RCLI_OBT_RARORACxSECTOR(System.Web.HttpContext.Current.Session["CODSECTORCliente"].ToString());

            Double nuevoRARORAC = simularBL.SimularRARORAC(objCartera, Convert.ToDouble(dt.Rows[0]["IN_UNE"].ToString()), Convert.ToDouble(dt.Rows[0]["PROV"].ToString()), Convert.ToDouble(dt.Rows[0]["IN_PE"].ToString()), Convert.ToDouble(dt.Rows[0]["IN_KE"].ToString()), delta);

            RARORACpercent = String.Format("{0:0.0}%", nuevoRARORAC * 100);

            //realCARTERA.Text = String.Format("{0:0.0}%", nuevoRARORAC * 100);

            //txtMoverRARORAC.Text = txtMoverRARORAC.Text + "%";
        }
        public void SimularRARORAC2()
        {
            RARORACBL simularBL = new RARORACBL();

            Double delta = 0;

            String numero = txtMoverRARORAC.Text.Replace("%", "");
            //if (numero == "") numero = "0";

            delta = Convert.ToDouble(numero);

            delta = delta / 100;

            dt = Session["dt_Cliente"] as DataTable;

            if (!String.IsNullOrEmpty(txt_cliente.Text))
            {
                dv = dt.DefaultView;
                dv.RowFilter = "[CLIENTE] LIKE '" + txt_cliente.Text + "' OR [IDC] LIKE '%" + txt_cliente.Text + "%'";
            }

            dataCliente = dv.ToTable();

            ListaDatoClienteBL raroracCartera = new ListaDatoClienteBL();
            objCartera = raroracCartera.RCLI_OBT_RARORACxSECTOR(dataCliente.Rows[0]["CODSECTOR"].ToString());

            Double nuevoRARORAC = simularBL.SimularRARORAC(objCartera, Convert.ToDouble(dataCliente.Rows[0]["IN_UNE"].ToString()), Convert.ToDouble(dataCliente.Rows[0]["PROV"].ToString()), Convert.ToDouble(dataCliente.Rows[0]["IN_PE"].ToString()), Convert.ToDouble(dataCliente.Rows[0]["IN_KE"].ToString()), delta);
            realCARTERA.Text = String.Format("{0:0.0}%", nuevoRARORAC * 100);

            txtMoverRARORAC.Text = txtMoverRARORAC.Text.Replace("%", "") + "%";
        }
        public void txtMoverRARORAC_TextChanged(object sender, EventArgs e)
        {
            if (txtMoverRARORAC.Text == "" || txtMoverRARORAC.Text == "%") txtMoverRARORAC.Text = "0%";

            SimularRARORAC2();
        }

        protected void gvDepositosUsol_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataTable tb = new DataTable();
            tb = Session["dt_tiempo_depositos"] as DataTable;
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Text = tb.Rows[0][2].ToString();
                e.Row.Cells[2].Text = tb.Rows[1][2].ToString();
                e.Row.Cells[3].Text = tb.Rows[2][2].ToString();
                e.Row.Cells[4].Text = tb.Rows[3][2].ToString();
                e.Row.Cells[5].Text = tb.Rows[4][2].ToString();
                e.Row.Cells[6].Text = tb.Rows[5][2].ToString();
                e.Row.Cells[7].Text = tb.Rows[6][2].ToString();
                e.Row.Cells[8].Text = tb.Rows[7][2].ToString();
                e.Row.Cells[9].Text = tb.Rows[8][2].ToString();
                e.Row.Cells[10].Text = tb.Rows[9][2].ToString();
                e.Row.Cells[11].Text = tb.Rows[10][2].ToString();
                e.Row.Cells[12].Text = tb.Rows[11][2].ToString();
                e.Row.Cells[13].Text = tb.Rows[12][2].ToString();
            }
        }

        protected void gvDepositosUs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataTable tb = new DataTable();
            tb = Session["dt_tiempo_depositos"] as DataTable;
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Text = tb.Rows[0][2].ToString();
                e.Row.Cells[2].Text = tb.Rows[1][2].ToString();
                e.Row.Cells[3].Text = tb.Rows[2][2].ToString();
                e.Row.Cells[4].Text = tb.Rows[3][2].ToString();
                e.Row.Cells[5].Text = tb.Rows[4][2].ToString();
                e.Row.Cells[6].Text = tb.Rows[5][2].ToString();
                e.Row.Cells[7].Text = tb.Rows[6][2].ToString();
                e.Row.Cells[8].Text = tb.Rows[7][2].ToString();
                e.Row.Cells[9].Text = tb.Rows[8][2].ToString();
                e.Row.Cells[10].Text = tb.Rows[9][2].ToString();
                e.Row.Cells[11].Text = tb.Rows[10][2].ToString();
                e.Row.Cells[12].Text = tb.Rows[11][2].ToString();
                e.Row.Cells[13].Text = tb.Rows[12][2].ToString();
            }
        }



        protected void gvData1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataTable tb = new DataTable();
            tb = Session["dt_tiempo_indicadores_file"] as DataTable;
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[2].Text = tb.Rows[0][2].ToString(); 
                e.Row.Cells[3].Text = tb.Rows[1][2].ToString();
                e.Row.Cells[5].Text = "ACUM" + tb.Rows[1][2].ToString().Substring(tb.Rows[1][2].ToString().Length-2); //ACUM AÑO ACTUAL
  
                e.Row.Cells[6].Text = tb.Rows[2][2].ToString();
                e.Row.Cells[8].Text = "ACUM" + tb.Rows[2][2].ToString().Substring(tb.Rows[2][2].ToString().Length - 2); //ACUM AÑO ANTIGUA

                e.Row.Cells[10].Text = tb.Rows[0][2].ToString();
                e.Row.Cells[11].Text = tb.Rows[1][2].ToString();

            }
        }

        protected void btnDescargar_ArchivoSectorial_Click(object sender, CommandEventArgs e)
        {
            LinkButton lb = (LinkButton)sender;
            GridViewRow gvs = (GridViewRow)lb.NamingContainer;
            string ruta = string.Empty;
            string filename = string.Empty;
            int rowIndex = gvs.RowIndex;

            switch (e.CommandName)
            {
                case "ciiuFiltrado":

                    foreach (GridViewRow row in gv_CIIU.Rows)
                    {
                        string lbl_Ruta = (row.FindControl("lbl_Ruta") as Label).Text;
                        string lbl_NombreArchivo = (row.FindControl("lbl_NombreArchivo") as Label).Text;

                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            if (row.RowIndex == rowIndex)
                            {
                                ruta = lbl_Ruta.ToString();
                                filename = lbl_NombreArchivo.ToString();

                                DescargarArchivo(ruta, filename);
                            }
                        }
                    }
                    break;


                case "ciiuSectorEconomico":

                    foreach (GridViewRow row in gv_SectorEconomico.Rows)
                    {
                        string lbl_Ruta = (row.FindControl("lbl_RutaSE") as Label).Text;
                        string lbl_NombreArchivo = (row.FindControl("lbl_NombreArchivoSE") as Label).Text;

                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            if (row.RowIndex == rowIndex)
                            {
                                ruta = lbl_Ruta.ToString();
                                filename = lbl_NombreArchivo.ToString();

                                DescargarArchivo(ruta, filename);
                            }
                        }
                    }

                    break;

                case "ciiuMacro":

                    foreach (GridViewRow row in gv_ciiuMacro.Rows)
                    {
                        string lbl_Ruta = (row.FindControl("lbl_RutaMacro") as Label).Text;
                        string lbl_NombreArchivo = (row.FindControl("lbl_NombreArchivoMacro") as Label).Text;

                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            if (row.RowIndex == rowIndex)
                            {
                                ruta = lbl_Ruta.ToString();
                                filename = lbl_NombreArchivo.ToString();

                                DescargarArchivo(ruta, filename);
                            }
                        }
                    }

                    break;
                case "ReporteSemanal":

                    foreach (GridViewRow row in gv_ReporteSemanales.Rows)
                    {
                        string lbl_Ruta = (row.FindControl("lbl_RutaSemanal") as Label).Text;
                        string lbl_NombreArchivo = (row.FindControl("lbl_NombreArchivoSemanal") as Label).Text;

                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            if (row.RowIndex == rowIndex)
                            {
                                ruta = lbl_Ruta.ToString();
                                filename = lbl_NombreArchivo.ToString();

                                DescargarArchivo(ruta, filename);
                            }
                        }
                    }

                    break;
            }

        }

        public void DescargarArchivo(String ruta, String filename)
        {
            System.IO.FileInfo toDownload = new System.IO.FileInfo(ruta);

            if (toDownload.Exists)
            {
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                Response.ContentType = "application/octet-stream";
                Response.WriteFile(ruta);
                Response.Flush();
                Response.Close();
                Response.End();

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            else
            {
                //
            }

        }

        public void MostrarArchivosSegunCIIU(DataTable datacliente)
        {
            string ciiu = string.Empty;
            ciiu = dataCliente.Rows[0]["CIIU"].ToString();
            DatoArchivoSectorial das = new DatoArchivoSectorial();
            das.ciiu = ciiu;
            DataSet ds = BCP.PERCLI.BUS.BLL.GestionRevalidacionCEMBL.instancia.mostrarArchivoSectorial(das);
            gv_CIIU.DataSource = ds.Tables[0];
            gv_CIIU.DataBind();
            if (ds.Tables[0].Rows.Count > 0)
            {
                lbl_SectorEconomico.Text = ds.Tables[0].Rows[0]["SECTORECONOMICO_SBS"].ToString();
                lbl_SectorEconomico.Font.Bold = true;
            }

        }

        public void MostrarArchivosMacro()
        {
            DataSet ds = BCP.PERCLI.BUS.BLL.GestionRevalidacionCEMBL.instancia.mostrarArchivoSectorialMacro();
            gv_ciiuMacro.DataSource = ds.Tables[0];
            gv_ciiuMacro.DataBind();

        }

        public void MostrarTop4Semanales()
        {
            ds = GestionRevalidacionCEMBL.instancia.mostrarArchivosSemanales(null, null);
            dt = ds.Tables[1];
            gv_ReporteSemanales.DataSource = dt;
            gv_ReporteSemanales.DataBind();
        }


        private void RegisterPostBackControl()
        {


            foreach (GridViewRow row in gv_SectorEconomico.Rows)
            {
                LinkButton lnkFull = row.FindControl("lnk_DescargarArchivoSE") as LinkButton;
                ScriptManager.GetCurrent(this).RegisterPostBackControl(lnkFull);
            }
        }

        protected void gv_RowCreated_ArchivosCIIU(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.CssClass = "header";

            }
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState == DataControlRowState.Normal)
            {
                e.Row.CssClass = "normal";
            }
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState == DataControlRowState.Alternate)
            {
                e.Row.CssClass = "alternate";
            }
        }

        protected void btn_RedireccionarRepositorio(object sender, EventArgs e)
        {
            string IDC;
            dt = Session["dt_Cliente"] as DataTable;

            //Busqueda de clientes
            if (!String.IsNullOrEmpty(txt_cliente.Text))
            {
                dv = dt.DefaultView;
                dv.RowFilter = "[CLIENTE] = '" + txt_cliente.Text + "' OR [IDC] = '" + txt_cliente.Text + "'";

                if (dv.ToTable().Rows.Count == 0)
                {
                    //Message
                    validacion = new StringBuilder();
                    validacion.Append("Por favor, seleccionar un cliente de la lista o ingresar un cliente de su cartera.");

                    //Advertencia                   
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "Validacion_Solicitud('Bienvenido a la Web Resumen Cliente');", true);
                }
                else
                {
                    IDC = dv.ToTable().Rows[0]["IDC"].ToString();
                    Response.Redirect("Archivos_2.aspx?IDC=" + IDC);
                }
            }
            else
            {
                //Message
                validacion = new StringBuilder();
                validacion.Append("Por favor, seleccionar un cliente de la lista o ingresar un cliente de su cartera.");

                //Advertencia                   
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "Validacion_Solicitud('Bienvenido a la Web Resumen Cliente');", true);
            }

        }

        public String imgIconArchivo(string ruta)
        {
            string urlImage = "../../img/icon_txt.png";
            string extensionArchivo = System.IO.Path.GetExtension(ruta);

            if (extensionArchivo == ".pdf")
            {
                urlImage = "../../img/icon_pdf_2.png";
            }
            else if (extensionArchivo == ".xls" || extensionArchivo == ".xlsx")
            {
                urlImage = "../../img/icon_excel.png";
            }
            else if (extensionArchivo == ".docx" || extensionArchivo == ".doc")
            {
                urlImage = "../../img/icon_word.png";
            }
            return urlImage;

        }

        protected void btn_RedireccionarSAFIC(object sender, EventArgs e)
        {
            string IDC;
            dt = Session["dt_Cliente"] as DataTable;

            //Busqueda de clientes
            if (!String.IsNullOrEmpty(txt_cliente.Text))
            {
                dv = dt.DefaultView;
                dv.RowFilter = "[CLIENTE] = '" + txt_cliente.Text + "' OR [IDC] = '" + txt_cliente.Text + "'";

                if (dv.ToTable().Rows.Count == 0)
                {
                    //Message
                    validacion = new StringBuilder();
                    validacion.Append("Por favor, seleccionar un cliente de la lista o ingresar un cliente de su cartera.");

                    //Advertencia                   
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "Validacion_Solicitud('Bienvenido a la Web Resumen Cliente');", true);
                }
                else
                {
                    IDC = dv.ToTable().Rows[0]["IDC"].ToString();
                    Response.Redirect("SAFIC.aspx?IDC=" + IDC);
                }
            }
            else
            {
                //Message
                validacion = new StringBuilder();
                validacion.Append("Por favor, seleccionar un cliente de la lista o ingresar un cliente de su cartera.");

                //Advertencia                   
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "Validacion_Solicitud('Bienvenido a la Web Resumen Cliente');", true);
            }

        }



        protected void CargarSectorEconomico()
        {
            DatoArchivoSectorial das = new DatoArchivoSectorial();
            das.ciiu = "TOTAL";
            DataSet ds = BCP.PERCLI.BUS.BLL.GestionRevalidacionCEMBL.instancia.CargarSectoresEconomicos();
            dt = ds.Tables[0];
            ddlSectorEconomico.DataSource = dt;
            ddlSectorEconomico.DataValueField = "GRUPO";
            ddlSectorEconomico.DataTextField = "SECTORECONOMICO_SBS";
            ddlSectorEconomico.DataBind();
            ddlSectorEconomico.Items.Insert(0, "Seleccione Sector Económico");
        }

        protected void ddlSectorEconomico_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Para que no se dispare el evento cuando diga "Seleccione Sector Económico"
            if (ddlSectorEconomico.SelectedIndex != 0)
            {
                DatoArchivoSectorial das = new DatoArchivoSectorial();
                das.sectorEconomico = ddlSectorEconomico.SelectedValue;
                DataSet ds = BCP.PERCLI.BUS.BLL.GestionRevalidacionCEMBL.instancia.mostrarArchivoSectorEconomico(das);
                gv_SectorEconomico.DataSource = ds.Tables[0];
                gv_SectorEconomico.DataBind();
                this.RegisterPostBackControl();
            }
        }

        protected void btnConsultarReporteSemanal_Click(object sender, EventArgs e)
        {
            DateTime inicio = Convert.ToDateTime(dpFechaSemanalDe.Value);
            DateTime fin = Convert.ToDateTime(dpFechaSemanalAl.Value);

            ds = GestionRevalidacionCEMBL.instancia.mostrarArchivosSemanales(inicio, fin);
            dt = ds.Tables[0];
            gv_ReporteSemanales.DataSource = dt;
            gv_ReporteSemanales.DataBind();

            foreach (GridViewRow row in gv_ReporteSemanales.Rows)
            {
                LinkButton lnkFull = row.FindControl("lnk_DescargarArchivoSemanal") as LinkButton;
                ScriptManager.GetCurrent(this).RegisterPostBackControl(lnkFull);
            }
        }

        protected void ibtnRedireccionarMantenimientoSectoriales_Click(object sender, ImageClickEventArgs e)
        {
            if (objUsuario.Rol == "EECO" || objUsuario.Rol == "CRED" || objUsuario.Rol == "PROC" || objUsuario.Rol == "ADM")
            {
                Response.Redirect("~/MantenimientoSectoriales.aspx");
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "warningMessage", "toastr['error']('Su rol no cuenta con los permisos para realizar esta acción.');", true);
            }
        }

        protected void gvCuentaSueldo_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //Se oculta la columna 2 y 3
            e.Row.Cells[1].Visible = false;
            e.Row.Cells[2].Visible = false;
        }

        protected void gvCuentaSueldo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 0)
                {
                    for (int i = 3; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Text = FormatoNumDecimal(e.Row.Cells[i].Text, 2, 1);
                    }
                }
                if (e.Row.RowIndex == 1)
                {
                    for (int i = 3; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Text = FormatoNumDecimal(e.Row.Cells[i].Text, 0, 1);
                    }
                }
            }
            
        }






    }
}