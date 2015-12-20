// Auto-generated from tarimas.html.
// DO NOT EDIT.

library tarimas_html;

import 'dart:html' as autogenerated;
import 'dart:svg' as autogenerated_svg;
import 'package:web_ui/web_ui.dart' as autogenerated;
import 'package:web_ui/observe/observable.dart' as __observe;
import '_from_packages/widget/components/modal.dart';
import 'dart:html';
import 'dart:json' as JSON;
import 'package:web_ui/web_ui.dart';
import 'package:web_ui/watcher.dart' as watcher;
import 'package:widget/widget.dart';
import 'package:intl/intl.dart' as df;
import 'package:intl/number_format.dart' as nf;


// Original code



class Camara{
  String id;
  String nombre;
  Camara(this.id, this.nombre);
}

class DetalleReporteTarima{
  bool Seleccionada;
  int Folio;
  DateTime Fecha;
  
  String get FechaStr{
    try{
      return new df.DateFormat("dd/MMM/yyyy").format(this.Fecha);
    }
    catch(error){
      print("${error}");
      return "???";
    }
  }

  String Cajas;
  double Kilos;
  String KilosStr;
  
  String get Kstr{
     return new nf.NumberFormat("##,#").format(12.23);
  }
  
  String IP;
  String Lote;
  String Status;
  
  String get Estatus{
    if (this.Status == "C"){
      return "Cerrado";
    }
    if (this.Status == "S"){
      return "Embarcada";
    }
    else{
      return "En uso";
    }
  }
  
  String Camara;
  String Ubicacion;
  String FolioSalida;
  
  String get FolioSalidaStr{
    if (this.FolioSalida == "null"){
      return "";
    }
    else{
      return this.FolioSalida;
    }
  }
  
  DateTime FechaSalida;
  
  String get FechaSalidaStr{
    try{
      return new df.DateFormat("dd/MMM/yyyy").format(this.FechaSalida);
    }
    catch(error){
      return "";
    }
  }
  
  double PesoReal;
  String PesoRealStr;
}

int _lote = 1;
bool _todos = false;

String _fechaInicial = new df.DateFormat("dd/MM/yyyy").format(new DateTime.now());// new DateTime(new DateTime.now().year, new DateTime.now().month, new DateTime.now().day);
String _fechaFinal   = new df.DateFormat("dd/MM/yyyy").format(new DateTime.now());//new DateTime(new DateTime.now().year, new DateTime.now().month, new DateTime.now().day);
List<Camara> Camaras = new List<Camara>();
bool _camaras_todas = false;
List<DetalleReporteTarima> Reporte = new List<DetalleReporteTarima>();
int TotalCajas   = 0;
double TotalPeso = 0.0;

bool get todos_los_lotes{
  return _todos;
}

set todos_los_lotes(bool value){
  _todos = value;
  Element txtLote = query("#txtLote");
  if (value){
    txtLote.attributes["disabled"] = "disabled";
  }
  else{
    txtLote.attributes.remove("disabled");
  }
}

bool get camaras_todas{
  return _camaras_todas;
}

set camaras_todas(bool value){
  _camaras_todas = value;
  Element txtCamaras = query("#txtCamaras");
  if (value){
    txtCamaras.attributes["disabled"] = "disabled";
  }
  else{
    txtCamaras.attributes.remove("disabled");
  }
}

String get lote_tarima {
  try{
    return _lote.toString();
  }
  catch(error){
    return "0";
  }
}

set lote_tarima(value){
  try{
    _lote = int.parse(value);  
  }
  catch(error){
    _lote = 0;
  }
 }

llena_camaras(data){
  var pLista = JSON.parse(data);
  Camaras.clear();
  
  for(int i = 0; i<= pLista.length - 1; i ++){
    Camara pCamara = new Camara(pLista[i]["id"].toString(), pLista[i]["descripcion"]);
    Camaras.add(pCamara);
  }
  watcher.dispatch();
}

obtener_camaras(){
  HttpRequest.getString("/camaras/obtener_lista").then(llena_camaras);
}


genera_reporte(data){
  
  print("generando reporte...");
  
  var pLista = JSON.parse(data);
  print("items: ${pLista.length.toString()}");
  
  Reporte.clear();
  
  print("agregando items...");
  for (int i = 0; i <= pLista.length - 1; i ++){
    DetalleReporteTarima pDetalle = new DetalleReporteTarima();
    pDetalle.Cajas       = pLista[i]["cajas"].toString();
    pDetalle.Camara      = pLista[i]["contenedor"].toString();
    pDetalle.Kilos       = pLista[i]["kilos"];
    pDetalle.KilosStr    = pLista[i]["kilos_str"];
    pDetalle.Fecha       = DateTime.parse(pLista[i]["fecha"].substring(0, 10));
    try{
      pDetalle.FechaSalida = DateTime.parse(pLista[i]["fecha_salida"].substring(0, 10));
    }catch(error){
      pDetalle.FechaSalida = null;
    }
    pDetalle.Folio       = pLista[i]["folio"];
    pDetalle.FolioSalida = pLista[i]["id_salida"].toString();
    pDetalle.IP          = pLista[i]["ip"].toString();
    pDetalle.Lote        = pLista[i]["lote"].toString();
    pDetalle.PesoReal    = pLista[i]["pesoreal"];
    pDetalle.PesoRealStr = pLista[i]["pesoreal_str"];
    pDetalle.Status      = pLista[i]["estatus"].toString();
    pDetalle.Ubicacion   = pLista[i]["ubicacion"].toString();
    pDetalle.Seleccionada = false;
    
    TotalCajas++;
    TotalPeso += pDetalle.Kilos;
    
    Reporte.add(pDetalle);
  }

  watcher.dispatch();
}

obtener_reporte(){
  print("obteniendo reporte...");
  
  String pLoteIni = lote_tarima;
  String pLoteFin = lote_tarima;
  
  if (_todos){
    pLoteIni = "1";
    pLoteFin = "999";
  }
  
  InputElement e = new InputElement();
  String pCamaraIni = query("#txtCamaras").value;
  String pCamaraFin = pCamaraIni;
  
  if (_camaras_todas){
    pCamaraIni = "1";
    pCamaraFin = "999";
  }
  
  String pFechaIni = query("#txtFechaInicial").value;
  String pFechaFin = query("#txtFechaInicial").value;
  
  String pUrl = "/tarimas/lista?lote_ini=${pLoteIni}&lote_fin=${pLoteFin}&fecha_ini=${pFechaIni}&fecha_fin=${pFechaFin}&camara_ini=${pCamaraIni}&camara_fin=${pCamaraFin}";
  print("URL: ${pUrl}");

  HttpRequest.getString(pUrl).then(genera_reporte);
}

detalles(DetalleReporteTarima ADetalleTarima){
  print("detalles");
  query('#sId').text = ADetalleTarima.Folio.toString();
  query('#sNombre').text = ADetalleTarima.Seleccionada.toString();
  query('#modal_example').xtag.show();
}

seleccionar(DetalleReporteTarima ADetalleTarima){
  ADetalleTarima.Seleccionada = !ADetalleTarima.Seleccionada;
}

filtrar(){
  Reporte = Reporte.where((x) => x.Seleccionada == true).toList();
}


void main() {
  DivElement d = document.query("#demo");
  d.style.display = "block";
  
  query("#txtFechaInicial").value = new df.DateFormat("dd/MM/yyyy").format(new DateTime.now());
  query("#txtFechaFinal").value   = new df.DateFormat("dd/MM/yyyy").format(new DateTime.now());
  
  obtener_camaras();
}
// Additional generated code
void init_autogenerated() {
  var _root = autogenerated.document.body;
  final __html0 = new autogenerated.OptionElement(), __html1 = new autogenerated.Element.html('<tr>         \n           <td><input type="checkbox" value="Detalles" id="__e-4"></td>\n            <td id="__e-6"></td>\n            <td id="__e-8"></td>\n            <td id="__e-10"></td>\n            <td id="__e-12"></td>\n            <td id="__e-14"></td>\n            <td id="__e-16"></td>\n            <td id="__e-18"></td>\n            <td id="__e-20"></td>\n            <td id="__e-22"></td>\n            <td id="__e-24"></td>\n            <td id="__e-26"></td>\n            <td id="__e-28"></td>\n            <td><input type="button" value="Detalles" id="__e-29"></td>\n          </tr>');
  var __btnAceptar, __btnFiltrar, __demo, __e0, __e3, __e31, __e33, __e35, __modal_example, __myModalLabel, __sDepto, __sId, __sNombre, __txtCamaras, __txtFechaFinal, __txtFechaInicial, __txtLote;
  var __t = new autogenerated.Template(_root);
  __txtLote = _root.query('#txtLote');
  __t.listen(__txtLote.onInput, ($event) { lote_tarima = __txtLote.value; });
  __t.oneWayBind(() => lote_tarima, (e) { __txtLote.value = e; }, false, false);
  __e0 = _root.query('#__e-0');
  __t.listen(__e0.onChange, ($event) { todos_los_lotes = __e0.checked; });
  __t.oneWayBind(() => todos_los_lotes, (e) { __e0.checked = e; }, false, false);
  __txtFechaInicial = _root.query('#txtFechaInicial');
  __txtFechaFinal = _root.query('#txtFechaFinal');
  __txtCamaras = _root.query('#txtCamaras');
  __t.loop(__txtCamaras, () => Camaras, (camara, __t) {
    var __e2;
    __e2 = __html0.clone(true);
    var __binding1 = __t.contentBind(() => camara.nombre, false);
    __e2.nodes.add(__binding1);
    __t.oneWayBind(() => camara.id, (e) { __e2.value = e; }, false, false);
  __t.addAll([new autogenerated.Text('\n          '),
      __e2,
      new autogenerated.Text('\n        ')]);
  }, isTemplateElement: false);
  __e3 = _root.query('#__e-3');
  __t.listen(__e3.onChange, ($event) { camaras_todas = __e3.checked; });
  __t.oneWayBind(() => camaras_todas, (e) { __e3.checked = e; }, false, false);
  __btnAceptar = _root.query('#btnAceptar');
  __t.listen(__btnAceptar.onClick, ($event) { obtener_reporte(); });
  __btnFiltrar = _root.query('#btnFiltrar');
  __t.listen(__btnFiltrar.onClick, ($event) { filtrar(); });
  __e31 = _root.query('#__e-31');
  __t.loop(__e31, () => Reporte, (item, __t) {
    var __e10, __e12, __e14, __e16, __e18, __e20, __e22, __e24, __e26, __e28, __e29, __e30, __e4, __e6, __e8;
    __e30 = __html1.clone(true);
    __e4 = __e30.query('#__e-4');
    __t.listen(__e4.onClick, ($event) { seleccionar(item); });
    __e6 = __e30.query('#__e-6');
    var __binding5 = __t.contentBind(() => item.Folio, false);
    __e6.nodes.add(__binding5);
    __e8 = __e30.query('#__e-8');
    var __binding7 = __t.contentBind(() => item.FechaStr, false);
    __e8.nodes.add(__binding7);
    __e10 = __e30.query('#__e-10');
    var __binding9 = __t.contentBind(() => item.Cajas, false);
    __e10.nodes.add(__binding9);
    __e12 = __e30.query('#__e-12');
    var __binding11 = __t.contentBind(() => item.KilosStr, false);
    __e12.nodes.add(__binding11);
    __e14 = __e30.query('#__e-14');
    var __binding13 = __t.contentBind(() => item.IP, false);
    __e14.nodes.add(__binding13);
    __e16 = __e30.query('#__e-16');
    var __binding15 = __t.contentBind(() => item.Lote, false);
    __e16.nodes.add(__binding15);
    __e18 = __e30.query('#__e-18');
    var __binding17 = __t.contentBind(() => item.Estatus, false);
    __e18.nodes.add(__binding17);
    __e20 = __e30.query('#__e-20');
    var __binding19 = __t.contentBind(() => item.Camara, false);
    __e20.nodes.add(__binding19);
    __e22 = __e30.query('#__e-22');
    var __binding21 = __t.contentBind(() => item.Ubicacion, false);
    __e22.nodes.add(__binding21);
    __e24 = __e30.query('#__e-24');
    var __binding23 = __t.contentBind(() => item.FolioSalidaStr, false);
    __e24.nodes.add(__binding23);
    __e26 = __e30.query('#__e-26');
    var __binding25 = __t.contentBind(() => item.FechaSalidaStr, false);
    __e26.nodes.add(__binding25);
    __e28 = __e30.query('#__e-28');
    var __binding27 = __t.contentBind(() => item.PesoRealStr, false);
    __e28.nodes.add(__binding27);
    __e29 = __e30.query('#__e-29');
    __t.listen(__e29.onClick, ($event) { detalles(item); });
  __t.addAll([new autogenerated.Text('\n          '),
      __e30,
      new autogenerated.Text('  \n       ')]);
  }, isTemplateElement: false);
  __e33 = _root.query('#__e-33');
  var __binding32 = __t.contentBind(() => TotalCajas, false);
  __e33.nodes.add(__binding32);
  __e35 = _root.query('#__e-35');
  var __binding34 = __t.contentBind(() => TotalPeso, false);
  __e35.nodes.add(__binding34);
  __demo = _root.query('#demo');
  __modal_example = __demo.query('#modal_example');
  __myModalLabel = __modal_example.query('#myModalLabel');
  __sId = __modal_example.query('#sId');
  __sNombre = __modal_example.query('#sNombre');
  __sDepto = __modal_example.query('#sDepto');
  __t.component(new Modal.forElement(__modal_example));
  __t.create();
  __t.insert();
}

//@ sourceMappingURL=tarimas.dart.map