// Auto-generated from clases.html.
// DO NOT EDIT.

library clases_html;

import 'dart:html' as autogenerated;
import 'dart:svg' as autogenerated_svg;
import 'package:web_ui/web_ui.dart' as autogenerated;
import 'package:web_ui/observe/observable.dart' as __observe;
import 'dart:html';
import 'dart:json' as json;
import 'package:web_ui/web_ui.dart';
import 'package:web_ui/watcher.dart' as watcher;


// Original code


bool modo_insercion = true;

class Clase{
  int clave;
  String descripcion;
  String manejaextra;
  String tipo;
  int activo;
}

List<Clase> lista_clase = new List<Clase>();

llena_lista(data){
  lista_clase = new List<Clase>();
  List p_lista = json.parse(data);
 
  for (int i = 0; i <= p_lista.length - 1; i++){
    Clase item = new Clase();
    item.clave       = p_lista[i]["clave"];
    item.descripcion = p_lista[i]["descripcion"];
    item.manejaextra = p_lista[i]["manejaextra"];
    item.tipo        = p_lista[i]["tipo"];
    item.activo      = p_lista[i]["activo"];
    
    lista_clase.add(item);
  }
  
  watcher.dispatch();
}

obtener_lista(){
  HttpRequest.getString("/rest/usuarios/clase_obtener_lista").then(llena_lista);
}

Clase obtener_entidad(){
  Clase p_clase = new Clase();
  p_clase.clave       = int.parse(query("#clave").value);
  p_clase.descripcion = query("#descripcion").value;
  p_clase.manejaextra = query("#manejaextra").value;
  p_clase.tipo        = query("#tipo").value;
  p_clase.activo      = int.parse(query("#activo").value);
  
  return p_clase;
}

Map obtener_entidad_mapa(){
  Map p_map = new Map();
  p_map["clave"]       = query("#clave").value;
  p_map["descripcion"] = query("#descripcion").value;
  p_map["manejaextra"] = query("#manejaextra").value;
  p_map["tipo"]        = query("#tipo").value;
  p_map["activo"]      = query("#activo").value;
  
  return p_map;
}

limpia_edits(){
  query("#clave").value = "";
  query("#descripcion").value = "";
  query("#manejaextra").value = "";
  query("#tipo").value = "";
  query("#activo").value = "";
  
  query("#clave").focus();
}


agregar(){
  
  Clase p_clase = obtener_entidad();
  var req = new HttpRequest();
  
  if (query("#btnAgregar").value == "Agregar"){
    req.open("POST", "/rest/usuarios/clase_insertar");
    req.setRequestHeader("Content-type", "application/json");
    req.send(json.stringify(obtener_entidad_mapa()));
  
    lista_clase.add(obtener_entidad()); 
    limpia_edits();
  }
  else{
    req.open("POST", "/rest/usuarios/clase_actualizar");
    req.setRequestHeader("Content-type", "application/json");
    req.send(json.stringify(obtener_entidad_mapa()));
    limpia_edits();
    
    Clase p_clase_tmp = lista_clase.where((x) => x.clave == p_clase.clave).toList()[0];
    print("p_clase_tmp $p_clase_tmp");
    
    actualiza_clase(p_clase_tmp, p_clase);
    
    query("#btnAgregar").value = "Agregar";
  }
  
  modo_insercion = true;
}

void actualiza_clase(Clase p_clase_tmp, Clase p_clase) {
  p_clase_tmp.descripcion = p_clase.descripcion;
  p_clase_tmp.manejaextra = p_clase.manejaextra;
  p_clase_tmp.tipo        = p_clase.tipo;
  p_clase_tmp.activo      = p_clase.activo;
}

eliminar(Clase a_clase){
  if (window.confirm("¿Eliminar Clase ${a_clase.descripcion}?")){
  
  var req = new HttpRequest();
    req.open("POST", "/rest/usuarios/clase_eliminar");
    req.setRequestHeader("Content-type", "application/json");
    
    Map p_map = new Map();
    p_map["clave"] = a_clase.clave;
    
    req.send(json.stringify(p_map));
  
    lista_clase.remove(a_clase);
    query("#clave").focus();
  }
}

cancelar(){
  limpia_edits();
  modo_insercion = true;
}

despliga_clase(Clase a_clase){
  query("#clave").value       = a_clase.clave.toString();
  query("#descripcion").value = a_clase.descripcion;
  query("#manejaextra").value = a_clase.manejaextra;
  query("#tipo").value        = a_clase.tipo;
  query("#activo").value      = a_clase.activo.toString();
  
  query("#descripcion").focus();
  query("#btnAgregar").value = "Editar";
}

editar(Clase a_clase){
  despliga_clase(a_clase);
  modo_insercion = false;
  query("#btnAgregar").value == "Agregar";
}

void main() {
  obtener_lista();
}   

// Additional generated code
void init_autogenerated() {
  var _root = autogenerated.document.body;
  final __html0 = new autogenerated.Element.html('<th template="" instantiate="if modo_insercion"></th>'), __html1 = new autogenerated.Element.html('<input type="button" id="btnCancelar" value="Cancelar">'), __html2 = new autogenerated.Element.html('<tr style="display:none"></tr>'), __html3 = new autogenerated.Element.html('<tr template="" instantiate="if modo_insercion">\n              <td id="__e-3"></td>\n              <td id="__e-5"></td>\n              <td id="__e-7"></td>\n              <td id="__e-9"></td>\n              <td id="__e-11"></td>\n              <td id="__e-13" style="display:none"></td>\n            </tr>'), __html4 = new autogenerated.Element.html('<td template="" instantiate="if modo_insercion"><input type="button" id="btnEliminar" value="Eliminar"><input type="button" id="btnEditar" value="Editar"></td>');
  var __activo, __btnAgregar, __clave, __descripcion, __e0, __e1, __e16, __manejaextra, __sample_container_id, __tipo;
  var __t = new autogenerated.Template(_root);
  __sample_container_id = _root.query('#sample_container_id');
  __e0 = __sample_container_id.query('#__e-0');
  __t.conditional(__e0, () => modo_insercion, (__t) {
  __t.add(__html0.clone(true));
  });

  __clave = __sample_container_id.query('#clave');
  __descripcion = __sample_container_id.query('#descripcion');
  __manejaextra = __sample_container_id.query('#manejaextra');
  __tipo = __sample_container_id.query('#tipo');
  __activo = __sample_container_id.query('#activo');
  __btnAgregar = __sample_container_id.query('#btnAgregar');
  __t.listen(__btnAgregar.onClick, ($event) { agregar(); });
  __e1 = __sample_container_id.query('#__e-1');
  __t.conditional(__e1, () => !modo_insercion, (__t) {
    var __btnCancelar;
    __btnCancelar = __html1.clone(true);
    __t.listen(__btnCancelar.onClick, ($event) { cancelar(); });
  __t.add(__btnCancelar);
  });

  __e16 = __sample_container_id.query('#__e-16');
  __t.loop(__e16, () => lista_clase, (item, __t) {
    var __e15;
    __e15 = __html2.clone(true);
    __t.conditional(__e15, () => modo_insercion, (__t) {
      var __e11, __e13, __e14, __e3, __e5, __e7, __e9;
      __e14 = __html3.clone(true);
      __e3 = __e14.query('#__e-3');
      var __binding2 = __t.contentBind(() => item.clave, false);
      __e3.nodes.add(__binding2);
      __e5 = __e14.query('#__e-5');
      var __binding4 = __t.contentBind(() => item.descripcion, false);
      __e5.nodes.add(__binding4);
      __e7 = __e14.query('#__e-7');
      var __binding6 = __t.contentBind(() => item.manejaextra, false);
      __e7.nodes.add(__binding6);
      __e9 = __e14.query('#__e-9');
      var __binding8 = __t.contentBind(() => item.tipo, false);
      __e9.nodes.add(__binding8);
      __e11 = __e14.query('#__e-11');
      var __binding10 = __t.contentBind(() => item.activo, false);
      __e11.nodes.add(__binding10);
      __e13 = __e14.query('#__e-13');
      __t.conditional(__e13, () => modo_insercion, (__t) {
        var __btnEditar, __btnEliminar, __e12;
        __e12 = __html4.clone(true);
        __btnEliminar = __e12.query('#btnEliminar');
        __t.listen(__btnEliminar.onClick, ($event) { eliminar(item); });
        __btnEditar = __e12.query('#btnEditar');
        __t.listen(__btnEditar.onClick, ($event) { editar(item); });
      __t.add(__e12);
      });

    __t.add(__e14);
    });

  __t.addAll([new autogenerated.Text('\n            '),
      __e15,
      new autogenerated.Text('\n          ')]);
  }, isTemplateElement: false);
  __t.create();
  __t.insert();
}

//@ sourceMappingURL=clases.dart.map