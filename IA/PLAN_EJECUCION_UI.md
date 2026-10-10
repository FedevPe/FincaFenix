# Desarrollo del frontend del sistema de gestión agrícola

## 1. Objetivo

Ha llegado el momento de comenzar a desarrollar la interfaz de usuario del sistema.

Quiero construir un frontend moderno, profesional, intuitivo y consistente, orientado a un sistema de gestión empresarial agrícola. La interfaz debe facilitar el acceso a los distintos módulos, la consulta de información operativa y la ejecución de las tareas habituales de los usuarios.

El frontend será una **Single Page Application (SPA)** desarrollada con React, con una arquitectura modular, componentes reutilizables y una estructura que permita incorporar nuevas funcionalidades progresivamente.

Antes de comenzar a implementar, analiza el repositorio y el contexto existente del proyecto. Identifica las tecnologías, convenciones, estructuras y funcionalidades ya implementadas que puedan influir en el desarrollo del frontend.

No comiences a modificar archivos hasta haber realizado este análisis y resuelto las dudas funcionales o técnicas que puedan afectar significativamente a la arquitectura o al diseño.

## 2. Tecnologías y herramientas

Las tecnologías principales que se deben utilizar son:

- **React:** desarrollo de la interfaz de usuario como SPA.
- **Font Awesome:** biblioteca exclusiva para los iconos de la interfaz.
- **TypeScript:** utilizarlo si no existe una restricción técnica que lo impida, priorizando el tipado explícito y la seguridad de tipos.
- **CSS:** utilizar una estrategia de estilos coherente con la arquitectura existente. Si el proyecto ya cuenta con una solución de estilos, evaluar su reutilización antes de incorporar dependencias adicionales.

Puedes proponer otras dependencias cuando aporten un beneficio concreto, por ejemplo para el enrutamiento, la gestión de formularios, las tablas de datos o la visualización de gráficos. Sin embargo, explica brevemente su propósito y evita agregar dependencias innecesarias.

No reemplaces tecnologías o configuraciones existentes sin una justificación técnica.

## 3. Arquitectura general de la interfaz

### 3.1. Aplicación SPA y navegación

La aplicación debe funcionar como una SPA, con navegación entre páginas sin recargas completas del documento.

Cada módulo principal tendrá su propia página y podrá contener subpáginas o vistas especializadas.

Debe existir una página principal que funcione como dashboard, desde la cual el usuario pueda consultar información relevante y acceder a los distintos módulos del sistema.

Implementa un sistema de enrutamiento apropiado para React. Las rutas deben ser claras, consistentes y preparadas para incorporar nuevas páginas.

La estructura debe diferenciar claramente:

- El layout general de la aplicación autenticada.
- El layout de las páginas públicas, como el login.
- Las páginas de cada módulo.
- Los componentes compartidos.
- Los componentes específicos de cada módulo.
- Los servicios de acceso a datos y autenticación.
- Los modelos y tipos de datos.

Evita concentrar toda la interfaz y la lógica en un único componente.

### 3.2. Layout de autenticación (AuthLayout)

La aplicación debe contar con un layout independiente para las páginas públicas relacionadas con la autenticación.

Inicialmente, este layout se utilizará para la página de login.

Debe tener un diseño visual propio, sin mostrar el menú lateral ni el AppBar de la aplicación autenticada.

Su implementación debe permitir incorporar en el futuro otras páginas públicas de autenticación, si los requisitos del sistema lo justifican.

### 3.3. Layout principal de la aplicación (AppLayout)

El layout principal será utilizado por el dashboard y los módulos privados del sistema.

Debe integrar los siguientes elementos:

- Menú lateral izquierdo.
- AppBar superior.
- Área principal de contenido.
- Sistema global de temas.
- Navegación entre módulos y subpáginas.

El layout debe mantener una estructura visual consistente, independientemente del módulo que se esté utilizando.

Los componentes de navegación deben ser reutilizables y no deben necesitar implementaciones diferentes para cada página.

### 3.4. Menú lateral

La aplicación debe contar con un menú lateral izquierdo que permita navegar entre los módulos disponibles.

El menú debe:

- Mostrar un icono Font Awesome y el nombre de cada módulo.
- Destacar visualmente el módulo o la ruta activa.
- Permitir contraer y expandir el menú para optimizar el espacio de trabajo.
- Mantener una navegación clara y consistente.
- Adaptarse a diferentes resoluciones de pantalla.
- Utilizar los mismos criterios visuales en todas las páginas.

Los módulos deben definirse de forma centralizada para facilitar la incorporación de nuevas opciones.

No agregues módulos ficticios como si fueran funcionalidades implementadas. Si es necesario mostrar accesos futuros, deben identificarse explícitamente como pendientes o utilizarse únicamente como elementos de diseño provisionales.

### 3.5. AppBar

En la parte superior de la aplicación debe existir una barra de navegación que incluya:

- El nombre del módulo o sección actual.
- La información disponible del usuario autenticado.
- Un menú de usuario con las acciones que permita la infraestructura existente.
- Un control para alternar entre tema claro y oscuro.
- Los controles adicionales que sean necesarios para mejorar la navegación.

La información del usuario debe obtenerse de la fuente de autenticación existente.

El título de la sección actual debe mantenerse sincronizado con la ruta de navegación.

El AppBar solo debe mostrarse dentro del layout de la aplicación autenticada.

### 3.6. Sistema de pestañas

Algunos módulos tendrán páginas con múltiples vistas o subfuncionalidades.

En esos casos, debe existir una barra de pestañas que permita cambiar entre las distintas secciones sin abandonar el contexto de la página actual.

Por ejemplo, en los detalles de una orden de trabajo, el usuario podrá navegar entre actividades, recetas, consumos e informes.

Las pestañas deben:

- Indicar claramente cuál está activa.
- Mantener una apariencia consistente en toda la aplicación.
- Mostrar únicamente las opciones que correspondan al contexto actual.
- Evitar recargas completas de la página.
- Permitir incorporar nuevas secciones sin modificar innecesariamente los componentes existentes.

Cuando sea apropiado, la pestaña activa debe estar representada en la URL para permitir compartir enlaces directos y conservar la navegación al recargar la página.

No utilices pestañas para sustituir la navegación entre módulos independientes.

## 4. Página de inicio de sesión (Login)

### 4.1. Objetivo y diseño visual

La aplicación debe contar con una página de inicio de sesión que permita a los usuarios autenticarse antes de acceder a las funcionalidades privadas del sistema.

La página debe ofrecer una interfaz profesional, limpia y minimalista, con una identidad visual coherente con el resto de la aplicación.

El diseño debe priorizar la claridad y la facilidad de uso, evitando elementos decorativos innecesarios.

Debe contemplar:

- Identidad del sistema, incluyendo logotipo si existe uno en el proyecto.
- Campo para ingresar el nombre de usuario o correo electrónico, según el mecanismo de autenticación existente.
- Campo para ingresar la contraseña.
- Control para mostrar u ocultar la contraseña.
- Botón para iniciar sesión.
- Indicador visual durante el proceso de autenticación.
- Mensajes de error claros cuando las credenciales sean incorrectas o se produzca un problema de comunicación.
- Validaciones básicas de los campos obligatorios.
- Accesibilidad mediante teclado, etiquetas y estados de foco visibles.

No agregues funcionalidades como registro de usuarios, recuperación de contraseña o autenticación multifactor si no forman parte de los requisitos existentes. Si alguna de ellas ya está implementada en el backend, evalúa cómo integrarla.

La página debe adaptarse a distintas resoluciones de pantalla y respetar los temas claro y oscuro de la aplicación.

### 4.2. Integración con la autenticación

Antes de implementar el login, inspecciona el mecanismo de autenticación existente en el backend y determina cómo debe integrarse con React.

Identifica específicamente:

- El endpoint de autenticación disponible.
- El formato de las credenciales y de la respuesta.
- El mecanismo utilizado para mantener la sesión: cookies, tokens u otro.
- Los mecanismos de autorización y los roles o permisos disponibles.
- El procedimiento para cerrar sesión.
- El comportamiento esperado cuando la sesión expira o deja de ser válida.

Reutiliza los mecanismos existentes y evita implementar un sistema de autenticación paralelo.

No almacenes contraseñas ni información sensible en el navegador.

Si se utilizan cookies de sesión, respeta sus atributos de seguridad y la estrategia de protección contra CSRF que corresponda.

Si se utilizan tokens, define su almacenamiento y renovación de acuerdo con la arquitectura de seguridad existente. Evita guardar tokens sensibles en `localStorage` sin una evaluación explícita de los riesgos.

Si el backend todavía no dispone de un mecanismo de autenticación compatible con el frontend, documenta qué componentes faltan y propone una solución antes de modificar la arquitectura existente.

### 4.3. Flujo de autenticación y navegación

El comportamiento esperado es el siguiente:

1. Cuando un usuario no autenticado ingrese a la aplicación, debe ser redirigido a la página de login.
2. Cuando las credenciales sean válidas, debe acceder al dashboard principal.
3. Si el usuario intenta acceder directamente a una ruta privada sin estar autenticado, debe ser redirigido al login.
4. Si el usuario accede al login estando autenticado, debe ser redirigido al dashboard o a la ruta de destino correspondiente.
5. Si se produce una expiración de sesión, la aplicación debe gestionar la situación de forma consistente, informar al usuario cuando corresponda y solicitar una nueva autenticación.
6. Cuando el usuario cierre sesión, debe invalidarse la sesión mediante el mecanismo previsto por el backend y redirigirse al login.

Si el usuario fue redirigido al login al intentar acceder a una ruta específica, puede regresar a esa ruta después de autenticarse correctamente, siempre que continúe teniendo autorización para acceder a ella.

La autenticación y la autorización deben estar separadas de los componentes visuales.

Implementa una estrategia centralizada para gestionar la sesión y proteger las rutas, utilizando las capacidades del sistema de enrutamiento elegido.

La protección de las rutas en React es una medida de navegación y experiencia de usuario, no un mecanismo de seguridad suficiente por sí solo. El backend debe continuar validando la identidad y los permisos en cada operación protegida.

### 4.4. Gestión de la sesión

La aplicación debe disponer de una forma centralizada de consultar el estado de autenticación del usuario.

Como mínimo, deben contemplarse los siguientes estados:

- Inicializando o verificando la sesión.
- Usuario autenticado.
- Usuario no autenticado.
- Error al verificar o recuperar la sesión.

Durante la verificación inicial, evita mostrar brevemente páginas privadas a usuarios que todavía no hayan sido autenticados.

La información del usuario autenticado debe estar disponible para los componentes que la necesiten, especialmente el AppBar y los elementos de navegación condicionados por permisos.

No utilices el estado del frontend como única fuente de verdad para validar permisos o roles.

### 4.5. Cierre de sesión

La aplicación debe proporcionar una opción para cerrar sesión desde el menú del usuario en el AppBar.

El proceso debe:

1. Ejecutar el mecanismo de cierre de sesión previsto por el backend, si existe.
2. Limpiar el estado de autenticación del frontend.
3. Eliminar los datos temporales de sesión que corresponda.
4. Redirigir al usuario a la página de login.
5. Impedir que se acceda a páginas privadas mediante la navegación normal una vez cerrada la sesión.

No debe asumirse que ocultar las páginas en el frontend invalida una sesión que todavía permanezca activa en el servidor.

### 4.6. Separación entre autenticación y aplicación

La página de login no debe mostrar el menú lateral ni el AppBar de la aplicación autenticada.

Debe utilizar el `AuthLayout`, mientras que el dashboard y los módulos privados deben utilizar el `AppLayout`.

La organización del código debe contemplar una capa específica para autenticación, separada de los servicios de negocio.

Por ejemplo, pueden existir componentes o servicios para gestionar el estado de autenticación, proteger rutas, iniciar sesión, cerrar sesión y recuperar la información del usuario autenticado.

La estructura definitiva debe adaptarse a las convenciones del proyecto y al mecanismo de autenticación del backend.

## 5. Identidad visual y diseño

La interfaz debe transmitir una imagen profesional, moderna y sobria, adecuada para un sistema de gestión empresarial utilizado durante jornadas de trabajo.

El diseño debe priorizar la legibilidad, la densidad de información, la facilidad de uso y la consistencia visual por encima de los elementos decorativos.

### 5.1. Paleta de colores

Todas las páginas deben respetar una paleta de colores principal centralizada.

Antes de definirla, revisa si el proyecto ya cuenta con colores, logotipos, estilos o referencias visuales. Si existe una identidad visual establecida, reutilízala.

Si no existe, propone una paleta apropiada para un sistema de gestión agrícola, justificando brevemente la elección antes de aplicarla.

Centraliza los colores y demás valores visuales mediante variables CSS o tokens de diseño. Evita distribuir valores de color arbitrarios por los componentes.

La paleta debe contemplar, como mínimo:

- Color primario.
- Color secundario o de acento.
- Fondos principales y secundarios.
- Superficies de tarjetas, tablas, formularios y paneles.
- Colores de texto y texto secundario.
- Bordes y separadores.
- Estados de éxito, advertencia, error e información.
- Estados interactivos como hover, focus, selección y deshabilitado.

Los estados funcionales deben ser reconocibles incluso cuando el usuario no distinga los colores.

### 5.2. Temas claro y oscuro

El usuario debe poder alternar entre tema claro y tema oscuro desde el AppBar.

Ambos temas deben estar diseñados de forma completa y coherente. No debe tratarse simplemente de invertir el color de fondo.

El cambio de tema debe aplicarse globalmente a todos los módulos y componentes, incluyendo:

- Login.
- Menús.
- Tablas.
- Formularios.
- Pestañas.
- Diálogos.
- Gráficos.
- Notificaciones.
- Estados de interacción.

La preferencia seleccionada debe persistir entre sesiones del navegador mediante un mecanismo apropiado, como `localStorage`, dado que la preferencia visual no contiene por sí misma información sensible de autenticación.

Si no existe una preferencia guardada, se puede utilizar el tema del sistema operativo como valor inicial.

### 5.3. Tipografía e iconografía

Todos los iconos deben proceder de Font Awesome. No mezcles bibliotecas de iconos ni utilices emojis como sustitutos de iconos funcionales.

Selecciona el paquete de Font Awesome adecuado para React y utiliza componentes de iconos reutilizables cuando corresponda.

La tipografía debe ser legible y consistente, con una jerarquía visual clara para títulos, subtítulos, etiquetas, datos tabulares y textos auxiliares.

### 5.4. Componentes reutilizables

Diseña componentes compartidos para los elementos que se repitan en distintos módulos, por ejemplo:

- Botones y grupos de acciones.
- Campos de formularios.
- Selectores y filtros.
- Tablas de datos.
- Indicadores de estado.
- Tarjetas informativas.
- Paginación.
- Barras de pestañas.
- Diálogos de confirmación.
- Notificaciones y mensajes de error.
- Estados de carga, vacío y error.
- Componentes de autenticación.
- Elementos de navegación.

No es necesario construir un sistema de componentes excesivamente abstracto. Busca un equilibrio entre reutilización, simplicidad y facilidad de mantenimiento.

## 6. Dashboard principal

El dashboard será la página inicial de la aplicación después de una autenticación exitosa.

Su objetivo es ofrecer una visión general de la actividad del sistema y proporcionar accesos rápidos a los módulos disponibles.

El diseño debe contemplar espacios para:

- Indicadores operativos relevantes.
- Resúmenes de órdenes de trabajo según su estado.
- Actividad reciente, cuando existan datos disponibles.
- Accesos directos a las principales funcionalidades.
- Gráficos o estadísticas que aporten información útil para la gestión.

La selección definitiva de indicadores debe basarse en los datos y las reglas de negocio existentes.

No inventes métricas, resultados ni datos reales. Si el backend todavía no proporciona la información necesaria, identifica qué datos se necesitan y presenta únicamente estados vacíos, datos de demostración claramente identificados o componentes preparados para su integración.

El dashboard debe ser modular, de modo que sus indicadores puedan evolucionar sin necesidad de rediseñar toda la página.

Los accesos a módulos y las acciones disponibles deben respetar los permisos del usuario autenticado, sin sustituir las comprobaciones de autorización del backend.

## 7. Módulo de órdenes de trabajo

Este módulo será la primera funcionalidad que se desarrollará en profundidad y servirá como referencia para establecer los criterios de diseño y reutilización del resto de la aplicación.

El módulo debe incluir, como mínimo, una página de listado y una página de detalle.

### 7.1. Listado de órdenes de trabajo

La página principal del módulo debe permitir consultar las órdenes de trabajo existentes mediante una tabla de datos clara y funcional.

La tabla debe contemplar las siguientes columnas:

- Número de orden.
- Finca.
- Tarea.
- Estado.
- Fecha de inicio.
- Sectores de trabajo.
- Acciones disponibles.

Las acciones por orden deben contemplar, según los permisos y las funcionalidades existentes:

- Acceder al detalle de la orden.
- Cambiar rápidamente su estado.
- Descargar la orden en formato PDF.

El cambio de estado debe realizarse mediante una interacción clara, con confirmación cuando la operación lo requiera. Deben respetarse las transiciones de estado permitidas por las reglas de negocio existentes.

La descarga del PDF debe integrarse con el mecanismo existente del backend. No generes documentos ficticios ni simules que una descarga se ha completado si no existe una implementación funcional.

### 7.2. Filtros y búsqueda

El usuario debe poder filtrar las órdenes de trabajo por:

- Estado.
- Intervalo de fechas.
- Finca.
- Tarea.

También debe poder combinar filtros y restablecerlos fácilmente.

Los filtros deben tener una presentación compacta y clara, sin ocupar espacio innecesario en pantalla.

Si el volumen de órdenes lo requiere, incorpora paginación y evita cargar todos los registros de manera innecesaria.

Si el backend dispone de filtros, ordenamiento y paginación, reutiliza esas capacidades. Si no existen, identifica las limitaciones antes de implementar una solución local que pueda resultar ineficiente.

Los estados de la interfaz deben contemplar:

- Carga de información.
- Resultados disponibles.
- Ausencia de resultados.
- Errores de comunicación.
- Aplicación y eliminación de filtros.

### 7.3. Detalle de una orden de trabajo

Al seleccionar una orden, el usuario debe acceder a una página específica para consultar toda la información relacionada con ella.

La página debe dividirse en dos áreas principales.

**Primera sección: cabecera de la orden**

Debe mostrar de forma clara:

- Número de orden.
- Tarea asignada.
- Finca.
- Sectores de trabajo asociados.
- Descripción de la orden.
- Estado actual.
- Fechas relevantes disponibles.

La información debe presentarse con una jerarquía visual que permita identificar rápidamente los datos más importantes.

**Segunda sección: contenido organizado mediante pestañas**

Debajo de la cabecera debe existir una barra de pestañas que permita consultar las distintas áreas funcionales de la orden.

Las pestañas previstas son:

1. **Actividades:** consulta de las actividades registradas por los operarios, incluyendo la información disponible sobre fechas, responsables, sectores y tareas realizadas.
2. **Receta:** consulta de la receta de materiales asociada a la orden, si existe.
3. **Consumos por sector:** consulta de los consumos de materiales asociados a cada sector de trabajo.
4. **Consumos por material:** consulta consolidada de los materiales utilizados y sus cantidades, según la información registrada.
5. **Rendimiento:** presentación de los indicadores e informes de rendimiento que permita calcular el modelo de datos existente.

Estas pestañas representan las áreas funcionales esperadas, pero su implementación definitiva debe respetar la estructura real del dominio y los datos que proporciona el backend.

Si alguna sección no corresponde a una orden determinada, debe mostrarse un estado vacío explicativo o un mensaje adecuado. No se deben inventar registros para completar la interfaz.

Las tablas de actividades y consumos deben facilitar la consulta de información, con encabezados claros, formatos numéricos consistentes y totales cuando estén respaldados por datos fiables.

Los informes de rendimiento pueden incluir tarjetas de indicadores, tablas o gráficos, siempre que existan datos suficientes para calcularlos correctamente.

Evita calcular en el frontend valores que deban ser determinados por las reglas de negocio del backend.

## 8. Integración con el backend

Antes de desarrollar los servicios de acceso a datos, inspecciona el backend y documenta brevemente:

- Endpoints disponibles.
- Modelos de respuesta y DTO.
- Mecanismos de autenticación y autorización.
- Reglas de negocio relevantes para las órdenes de trabajo.
- Operaciones disponibles para cambiar estados.
- Mecanismos existentes para generar o descargar PDF.
- Endpoints de actividades, recetas, consumos e informes.

Reutiliza los contratos existentes. No modifiques el backend para adaptarlo al frontend sin justificar previamente la necesidad.

Si una funcionalidad de la interfaz no puede implementarse con los endpoints disponibles, documenta la limitación y propone la modificación mínima necesaria.

Organiza las llamadas HTTP en una capa de servicios separada de los componentes visuales. Centraliza la configuración de la API y la gestión de errores.

No incluyas credenciales, secretos ni tokens sensibles en el código fuente.

Si el backend todavía no está preparado para alguna sección, implementa únicamente la estructura visual necesaria y deja explícito qué integración falta. No mezcles datos ficticios con datos reales.

## 9. Experiencia de usuario y accesibilidad

La aplicación debe ser cómoda para un uso frecuente durante jornadas de trabajo.

Prioriza:

- Navegación predecible.
- Jerarquía visual clara.
- Acciones principales fáciles de localizar.
- Formularios comprensibles.
- Tablas legibles con grandes volúmenes de información.
- Mensajes de validación y error útiles.
- Confirmaciones para acciones destructivas o sensibles.
- Indicadores visuales de carga y procesamiento.
- Diseño adaptable a resoluciones de escritorio y tabletas.

Implementa criterios básicos de accesibilidad, incluyendo etiquetas en controles, navegación por teclado, foco visible y contraste suficiente en ambos temas.

No utilices únicamente colores para representar estados. Combina color con texto o iconografía.

En los formularios de autenticación y en el resto de la aplicación, los errores deben comunicarse de forma comprensible, sin revelar información sensible ni detalles internos innecesarios.

## 10. Estrategia de implementación

El desarrollo debe realizarse de forma incremental.

### Fase 1: análisis

Inspecciona el repositorio y determina:

- Estructura actual del frontend y backend.
- Tecnologías y versiones.
- Arquitectura y convenciones existentes.
- Contratos de API disponibles.
- Modelos relacionados con las órdenes de trabajo.
- Dependencias instaladas.
- Mecanismos de autenticación, sesión y autorización.
- Endpoints de login, logout y consulta del usuario autenticado, si existen.
- Posibles restricciones técnicas.

No modifiques archivos durante esta fase.

### Fase 2: propuesta de arquitectura y diseño

Presenta una propuesta concreta que incluya:

- Estructura de carpetas.
- Estrategia de enrutamiento.
- Separación entre `AuthLayout` y `AppLayout`.
- Flujo de autenticación y protección de rutas.
- Gestión de sesión y permisos.
- Sistema de temas.
- Paleta de colores propuesta o existente.
- Componentes reutilizables.
- Organización de servicios y modelos.
- Estrategia de integración con el backend.
- Alcance inicial del login, dashboard y módulo de órdenes.

Explica brevemente las decisiones relevantes y plantea las preguntas necesarias antes de comenzar a implementar.

### Fase 3: implementación de la base visual

Una vez aprobada la propuesta, implementa:

- Configuración inicial de React.
- Sistema de enrutamiento.
- `AuthLayout`.
- Página de login.
- Gestión centralizada de autenticación.
- Protección de rutas privadas.
- `AppLayout`.
- Menú lateral.
- AppBar.
- Sistema de temas claro y oscuro.
- Componentes compartidos.
- Estructura del dashboard.
- Estilos globales y diseño adaptable.

La integración del login debe utilizar el mecanismo de autenticación existente. No des por terminado este punto si el formulario solamente tiene una apariencia funcional, pero no autentica realmente al usuario.

El objetivo es establecer una base visual y arquitectónica sólida antes de desarrollar cada módulo.

### Fase 4: implementación del módulo de órdenes

Desarrolla progresivamente:

1. Listado y tabla de órdenes.
2. Filtros y búsqueda.
3. Navegación al detalle.
4. Cabecera del detalle.
5. Sistema de pestañas.
6. Vistas de actividades, recetas y consumos.
7. Informes de rendimiento.
8. Acciones de cambio de estado y descarga de PDF.
9. Manejo de errores y estados de carga.

Prioriza las funcionalidades según las capacidades reales del backend.

### Fase 5: validación

Al finalizar cada etapa:

- Comprueba que el proyecto compile.
- Ejecuta las pruebas disponibles.
- Corrige los errores detectados.
- Verifica que la navegación funcione.
- Comprueba ambos temas.
- Revisa el comportamiento de los filtros y las tablas.
- Comprueba que no se hayan roto funcionalidades existentes.
- Valida el inicio y cierre de sesión.
- Comprueba las redirecciones de rutas públicas y privadas.
- Verifica el comportamiento ante sesiones expiradas.
- Confirma que los controles implementados ejecuten las operaciones previstas.

No declares una funcionalidad como terminada si solamente está representada visualmente y sus acciones todavía no funcionan.

## 11. Forma de trabajo del agente

Trabaja como un desarrollador frontend senior responsable de diseñar una solución mantenible y alineada con el dominio del sistema.

Respeta las siguientes pautas:

- Analiza antes de modificar.
- Reutiliza la arquitectura y los componentes existentes cuando corresponda.
- No inventes reglas de negocio ni contratos de API.
- No agregues dependencias sin una razón concreta.
- No realices refactorizaciones ajenas al objetivo actual.
- No sobrescribas configuraciones ni archivos existentes sin revisar su contenido.
- Separa los componentes visuales de la lógica de acceso a datos.
- Mantén una nomenclatura coherente.
- Prioriza implementaciones simples, tipadas y mantenibles.
- Informa sobre las limitaciones que impidan completar una funcionalidad.
- Evita dejar botones, filtros o controles interactivos sin comportamiento funcional.
- No implementes autenticación ficticia para simular que el sistema está integrado.
- Respeta las reglas de autorización existentes y no confíes exclusivamente en las restricciones del frontend.

Si encuentras una ambigüedad que pueda cambiar significativamente la arquitectura, el modelo de navegación, la seguridad o las reglas de negocio, detente y consulta antes de continuar.

Para decisiones menores y reversibles, utiliza un criterio técnico razonable y documenta brevemente la decisión, sin interrumpir innecesariamente el trabajo.

## 12. Primer resultado esperado

En tu primera respuesta, quiero recibir únicamente el resultado del análisis inicial y la propuesta de implementación.

Incluye:

1. Resumen del estado actual del proyecto.
2. Tecnologías y arquitectura detectadas.
3. Mecanismos de autenticación y endpoints relacionados.
4. Endpoints y modelos relevantes para las órdenes de trabajo.
5. Propuesta de estructura del frontend.
6. Propuesta de diseño visual y sistema de temas.
7. Diseño propuesto para el login y el flujo de autenticación.
8. Plan de implementación por etapas.
9. Dudas o decisiones pendientes que necesiten mi confirmación.

**No implementes todavía.** Primero revisaremos la propuesta y resolveremos las dudas. Una vez aprobada, avanzaremos con la implementación por etapas.