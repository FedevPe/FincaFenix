# Especificación funcional — Módulo de Inventario

**Versión:** 0.5
**Estado:** En definición / diseño
**Sistema:** Fincas Fénix

---

## 1. Descripción

El módulo de Inventario tendrá como objetivo gestionar los materiales utilizados por la empresa, permitiendo conocer su disponibilidad, ubicación por unidad productiva, movimientos, costos, reservas y consumos.

El módulo deberá integrarse con las funcionalidades existentes de **Órdenes de Trabajo, Recetas y Consumos**, y deberá quedar preparado desde su diseño inicial para una futura incorporación del módulo de **Compras**.

El diseño deberá priorizar:

* Trazabilidad de movimientos.
* Conservación de información histórica.
* Integridad del stock.
* Separación entre stock físico, reservado y disponible.
* Trazabilidad de costos.
* Preparación para futuras integraciones.
* Posibilidad de incorporar nuevas funcionalidades sin una reestructuración significativa de la base de datos.

---

# 2. Objetivos

El módulo deberá permitir:

* Administrar los materiales utilizados por la empresa.
* Definir unidades de medida y características de los materiales.
* Controlar el stock disponible de cada material.
* Controlar el stock independientemente para cada Finca / Unidad Productiva.
* Consultar el stock consolidado de toda la empresa.
* Registrar movimientos de inventario.
* Mantener un historial de movimientos.
* Controlar consumos asociados a Órdenes de Trabajo.
* Reservar cantidades de materiales asociadas a Órdenes de Trabajo.
* Diferenciar stock físico, stock reservado y stock disponible.
* Definir stocks mínimos por material y Finca.
* Detectar materiales con stock bajo.
* Mantener información histórica de costos.
* Congelar el costo utilizado por una Orden de Trabajo al momento de su creación.
* Integrarse posteriormente con Compras.
* Permitir generar posteriormente informes económicos y de consumo.

El módulo **no será responsable del cálculo de rendimiento o productividad de las Órdenes de Trabajo**. Dichas funcionalidades pertenecerán al dominio de Órdenes de Trabajo / Productividad y serán implementadas posteriormente.

---

# 3. Concepto de Finca / Unidad Productiva

En el contexto de la empresa, lo que inicialmente podría denominarse "Depósito" representa realmente una **Finca o Unidad Productiva**.

Una empresa puede poseer múltiples unidades productivas:

```text
Finca A
Finca B
Finca C
```

Un mismo material puede encontrarse simultáneamente en varias Fincas.

Por este motivo, un material no deberá tener una única Finca asociada directamente.

La relación conceptual será:

```text
Material N ───────── N Finca
             │
             │
       StockPorFinca
```

---

# 4. Gestión de Materiales

El módulo deberá permitir administrar los materiales utilizados por la empresa.

Como mínimo, un material podrá contener:

* Identificador.
* Código.
* Nombre.
* Descripción.
* Unidad de medida.
* Categoría, si corresponde.
* Estado.
* Costo de referencia actual, si se determina necesario.
* Otros atributos propios del material.

Ejemplo:

```text
Material
--------------------------------
Código: FERT-001
Nombre: Fertilizante X
Unidad: Kilogramo
Estado: Activo
Costo referencia: $1.800
```

El costo almacenado directamente en `Material`, si finalmente se mantiene, **no deberá utilizarse como fuente histórica de costos**.

---

# 5. Unidades de medida

Cada material deberá poseer una unidad de medida base.

Ejemplos:

* Kilogramo.
* Litro.
* Unidad.
* Metro.
* Bolsa.
* Caja.

La unidad de medida deberá utilizarse consistentemente en:

* Stock.
* Reservas.
* Recetas.
* Consumos.
* Movimientos.
* Compras.
* Informes.

La posibilidad de implementar conversiones entre unidades podrá evaluarse posteriormente.

---

# 6. Stock por Finca

El stock deberá administrarse independientemente para cada Finca.

Para esto se utilizará conceptualmente una entidad intermedia:

```text
StockPorFinca
```

Esta entidad relacionará:

```text
Material
    │
    ├── StockPorFinca ── Finca A
    │
    ├── StockPorFinca ── Finca B
    │
    └── StockPorFinca ── Finca C
```

Como mínimo deberá contemplar:

* Material.
* Finca.
* Stock físico.
* Stock reservado.
* Stock disponible.
* Stock mínimo.

Deberá existir una restricción que impida múltiples registros para la misma combinación:

```text
MaterialId + FincaId
```

---

# 7. Estados del stock

A partir de esta versión se establece explícitamente que el inventario deberá distinguir tres conceptos diferentes:

```text
Stock físico
Stock reservado
Stock disponible
```

## 7.1 Stock físico

Representa la cantidad físicamente existente en la Finca.

Ejemplo:

```text
Material: Fertilizante X
Finca A
Stock físico: 500 kg
```

El stock físico solamente deberá modificarse como consecuencia de operaciones reales de inventario, tales como:

* Ingresos.
* Consumos.
* Ajustes.
* Transferencias.

La creación de una Orden de Trabajo **no deberá modificar el stock físico**.

---

## 7.2 Stock reservado

Representa una cantidad de stock físico que ya está comprometida para una o más Órdenes de Trabajo, pero que todavía no fue consumida físicamente.

Ejemplo:

```text
Stock físico:      500 kg
Stock reservado:   150 kg
```

Los 150 kg continúan físicamente en la Finca, pero ya no deberán considerarse disponibles para ser comprometidos por nuevas operaciones.

---

## 7.3 Stock disponible

El stock disponible representa la cantidad que todavía puede ser comprometida por nuevas operaciones.

La regla será:

```text
StockDisponible =
    StockFisico - StockReservado
```

Ejemplo:

```text
Stock físico:      500 kg
Stock reservado:   150 kg
--------------------------
Stock disponible:  350 kg
```

El stock disponible **no deberá almacenarse necesariamente como un valor independiente** si puede calcularse de forma segura a partir del stock físico y reservado.

Si técnicamente se decide persistirlo por razones de rendimiento, deberá tratarse como un valor derivado y mantenerse consistente mediante reglas transaccionales.

---

# 8. Reserva blanda de materiales

Se establece como decisión funcional que las cantidades planificadas mediante una receta deberán generar una **reserva blanda** al crear una Orden de Trabajo.

La reserva tendrá como finalidad impedir que el sistema considere disponible para otras Órdenes de Trabajo una cantidad que ya está comprometida por una orden existente.

La reserva:

* No representa consumo real.
* No disminuye el stock físico.
* Sí disminuye el stock disponible.
* Deberá estar asociada a la Orden de Trabajo que la originó.
* Deberá poder liberarse total o parcialmente.
* Deberá ajustarse cuando se registre el consumo real.

---

# 9. Creación de una Orden de Trabajo con Receta

Cuando se cree una Orden de Trabajo asociada a una Receta, el sistema deberá procesar cada detalle de la receta.

Por ejemplo:

```text
Receta A

Fertilizante X → 100 kg
Producto Y     → 20 L
Producto Z     → 5 unidades
```

Al crear la Orden de Trabajo:

```text
OT #125

Reserva:
Fertilizante X → 100 kg
Producto Y     → 20 L
Producto Z     → 5 unidades
```

El sistema deberá:

1. Identificar los materiales incluidos en la receta.
2. Obtener las cantidades planificadas.
3. Identificar la Finca correspondiente a la Orden de Trabajo.
4. Verificar el stock disponible de cada material.
5. Crear las reservas correspondientes.
6. Asociar cada reserva con la Orden de Trabajo.
7. No descontar esas cantidades del stock físico.
8. Capturar el último costo registrado de cada material según la regla de costos definida en este documento.

---

# 10. Validación de disponibilidad para reservas

Al crear una Orden de Trabajo con receta, el sistema deberá verificar que exista suficiente stock disponible para generar las reservas.

La validación deberá utilizar:

```text
StockDisponible =
    StockFisico - StockReservado
```

Ejemplo:

```text
Stock físico:      100 kg
Stock reservado:    60 kg
Stock disponible:   40 kg
```

Si una nueva Orden de Trabajo requiere:

```text
50 kg
```

el sistema deberá rechazar la reserva porque:

```text
50 kg > 40 kg disponibles
```

El hecho de que existan físicamente 100 kg no deberá permitir reservar otros 50 kg.

De esta manera se evita que dos Órdenes de Trabajo comprometan simultáneamente la misma existencia disponible.

---

# 11. Consumo real y liberación de reservas

La reserva deberá mantenerse hasta que se registre el consumo real.

Ejemplo inicial:

```text
Stock físico:      500 kg
Stock reservado:   100 kg
Stock disponible:  400 kg
```

La Orden de Trabajo consume:

```text
70 kg
```

Luego del consumo:

```text
Stock físico:      430 kg
Stock reservado:    30 kg
Stock disponible:  400 kg
```

La lógica será:

```text
StockFisico -= CantidadConsumida
StockReservado -= CantidadConsumida
```

La cantidad reservada restante representa la parte de la planificación que todavía no fue consumida.

---

# 12. Consumo menor al reservado

Si una Orden de Trabajo tenía reservados:

```text
100 kg
```

pero finalmente consume:

```text
70 kg
```

los:

```text
30 kg
```

restantes deberán liberarse cuando la reserva de la Orden de Trabajo sea finalizada.

Resultado:

```text
Stock físico:      -70 kg
Stock reservado:   -100 kg
```

Conceptualmente:

```text
Reserva inicial:    100 kg
Consumo real:        70 kg
Reserva liberada:    30 kg
```

La liberación de la reserva deberá aumentar nuevamente el stock disponible para otras operaciones.

---

# 13. Consumo mayor al reservado

Inicialmente se establece que el consumo real no deberá poder superar automáticamente la cantidad reservada.

Ejemplo:

```text
Reservado: 100 kg
Consumo:   120 kg
```

El sistema deberá detectar que:

```text
120 kg > 100 kg
```

y deberá aplicar una regla de validación antes de confirmar el consumo.

La política exacta para permitir consumos superiores a la reserva deberá definirse posteriormente. Una alternativa sería permitirlo únicamente mediante una autorización o ampliación explícita de la reserva, siempre que exista stock disponible suficiente.

No deberá permitirse que un consumo superior a la reserva genere stock negativo de forma implícita.

---

# 14. Cancelación de una Orden de Trabajo

Si una Orden de Trabajo que posee reservas es cancelada antes de consumir los materiales, las reservas correspondientes deberán liberarse.

Ejemplo:

```text
Stock físico:      500 kg
Stock reservado:   100 kg
Stock disponible:  400 kg
```

Se cancela la Orden de Trabajo.

Resultado:

```text
Stock físico:      500 kg
Stock reservado:     0 kg
Stock disponible:  500 kg
```

La cancelación no deberá generar un consumo ni un movimiento físico de inventario.

---

# 15. Modificación de una Orden de Trabajo

La modificación de una Orden de Trabajo que afecte cantidades de una receta deberá actualizar las reservas correspondientes.

Ejemplo:

```text
Reserva original:
100 kg

Nueva cantidad planificada:
150 kg
```

El sistema deberá intentar incrementar la reserva en:

```text
50 kg
```

y deberá verificar nuevamente el stock disponible.

Si no existe suficiente stock disponible, la modificación deberá rechazarse o requerir una acción explícita definida por las reglas de negocio.

Si la cantidad planificada disminuye:

```text
100 kg → 70 kg
```

deberán liberarse:

```text
30 kg
```

de reserva.

---

# 16. Relación entre Receta, Reserva y Consumo

La relación conceptual será:

```text
Receta
   │
   │ cantidad planificada
   ▼
OrdenTrabajo
   │
   │ genera
   ▼
ReservaMaterial
   │
   │ consumo real
   ▼
Consumo
   │
   │ afecta
   ▼
MovimientoInventario
   │
   ▼
StockPorFinca
```

Esta separación es fundamental:

```text
Receta       = planificación
Reserva      = compromiso de stock
Consumo      = utilización real
Movimiento   = registro del cambio físico
Stock        = estado actual del inventario
```

---

# 17. Entidad conceptual de Reserva

Se deberá evaluar la incorporación de una entidad específica para representar las reservas.

Conceptualmente:

```text
ReservaMaterial
--------------------------------
Id
OrdenTrabajoId
MaterialId
FincaId
CantidadReservada
CantidadConsumida
CantidadPendiente
FechaCreacion
FechaLiberacion
Estado
```

Los campos definitivos deberán determinarse durante el diseño técnico.

Como mínimo, el modelo deberá permitir identificar:

* Qué material está reservado.
* En qué Finca.
* Para qué Orden de Trabajo.
* Qué cantidad fue reservada.
* Qué cantidad fue consumida.
* Qué cantidad continúa pendiente.
* Si la reserva está activa, liberada o cancelada.

La cantidad pendiente podrá calcularse conceptualmente como:

```text
CantidadPendiente =
    CantidadReservada - CantidadConsumida
```

---

# 18. Integridad de las reservas

Las reservas deberán gestionarse de forma transaccional.

No deberá ocurrir una situación en la que dos Órdenes de Trabajo puedan reservar simultáneamente la misma cantidad disponible como consecuencia de una condición de carrera.

Por ejemplo:

```text
Stock físico: 100 kg
Reservado:      0 kg
Disponible:   100 kg
```

OT #1 intenta reservar:

```text
80 kg
```

OT #2 intenta reservar simultáneamente:

```text
50 kg
```

El sistema deberá garantizar que no puedan confirmarse ambas reservas, ya que:

```text
80 + 50 = 130 kg
```

supera el stock físico disponible.

La implementación deberá utilizar mecanismos transaccionales y de concurrencia apropiados para Entity Framework Core / SQL Server.

---

# 19. Costos de los materiales

El sistema deberá distinguir entre:

### Costo actual o de referencia

Representa el último costo conocido del material.

### Costo histórico

Representa el costo asociado a una operación determinada.

### Costo utilizado por una Orden de Trabajo

Representa el costo que deberá utilizarse en los informes económicos de esa Orden de Trabajo.

Este último deberá quedar congelado al momento de crear la Orden de Trabajo.

---

# 20. Regla de costo histórico de las Órdenes de Trabajo

Cuando se cree una Orden de Trabajo, el sistema deberá obtener el **último costo registrado disponible en ese momento** para cada material utilizado por sus recetas.

El sistema deberá almacenar una instantánea histórica de dicho costo asociada a la Orden de Trabajo.

Las modificaciones posteriores del costo del material no deberán alterar el costo histórico de órdenes existentes.

### Regla de negocio

**RN-COSTO-ORDEN**

> Al crear una Orden de Trabajo, el sistema deberá obtener el último costo registrado para cada material utilizado por las recetas asociadas y almacenar dicho costo como costo histórico de la Orden. Las modificaciones posteriores del costo del material no deberán alterar el costo histórico de órdenes ya creadas.

---

# 21. Receta y costo histórico

La receta representa la composición planificada.

Por ejemplo:

```text
Receta A

Fertilizante X → 10 kg
Producto Y     → 2 L
```

Al crear:

```text
OT #125
```

el sistema deberá obtener los costos vigentes:

```text
Fertilizante X → $1.500/kg
Producto Y     → $800/L
```

y conservarlos en la información histórica de la Orden.

Posteriormente, si:

```text
Fertilizante X → $1.800/kg
Producto Y     → $950/L
```

la OT #125 deberá continuar utilizando:

```text
Fertilizante X → $1.500/kg
Producto Y     → $800/L
```

para sus informes históricos.

---

# 22. Receta vs. Consumo real

Deberá mantenerse estrictamente la distinción entre:

**Receta:** consumo planificado.

**Reserva:** cantidad de stock comprometida por la planificación.

**Consumo:** material efectivamente utilizado.

Ejemplo:

```text
Receta:
Fertilizante X → 100 kg

Reserva:
Fertilizante X → 100 kg

Consumo real:
Fertilizante X → 85 kg
```

La receta no deberá modificarse automáticamente para reflejar los 85 kg consumidos.

---

# 23. Rendimiento y productividad

El cálculo de rendimiento y productividad queda explícitamente **fuera del alcance del módulo de Inventario**.

No deberán incorporarse al modelo de Inventario entidades, reglas o cálculos cuyo objetivo principal sea determinar productividad laboral o rendimiento de tareas.

Este concepto será tratado posteriormente dentro del dominio de **Órdenes de Trabajo / Productividad**.

---

# 24. Rendimiento futuro de Órdenes de Trabajo

Como requisito futuro, una Orden de Trabajo podrá tener una tarea con una métrica de rendimiento.

Para Órdenes de Trabajo asociadas a recetas, el rendimiento podrá medirse utilizando la unidad correspondiente a la receta o a la actividad realizada.

Ejemplos:

```text
Hectáreas / hora
Kilogramos / hora
Metros / hora
Unidades / hora
```

Para Órdenes de Trabajo sin receta, el sistema podrá utilizar posteriormente las actividades registradas por los operarios y sus horas trabajadas como base para calcular rendimiento.

Ejemplo:

```text
Trabajo realizado: 240 metros
Horas trabajadas: 12 h

Rendimiento:
240 m / 12 h = 20 m/h
```

Esta funcionalidad queda registrada como **requisito futuro**, pero no deberá implementarse dentro del módulo de Inventario.

---

# 25. Movimientos de Inventario

Todo cambio físico del stock deberá quedar registrado mediante un movimiento de inventario.

Ejemplos:

* Ingreso por compra.
* Salida por consumo.
* Ajuste positivo.
* Ajuste negativo.
* Transferencia entre Fincas.

La creación o eliminación de una reserva **no deberá generar un movimiento físico de inventario**, porque la reserva no representa un movimiento material.

---

# 26. Fuente de verdad del stock

El sistema deberá distinguir:

```text
MovimientoInventario
        │
        │ historial de cambios físicos
        ▼
StockPorFinca
        │
        ├── StockFisico
        └── StockReservado
                │
                ▼
        StockDisponible
```

La reserva modifica el compromiso del stock, pero no el stock físico.

El consumo modifica tanto el stock físico como la reserva asociada.

Todas las modificaciones deberán realizarse de forma transaccional.

---

# 27. Reglas de integridad del stock

Como mínimo deberán cumplirse las siguientes reglas:

1. Un material puede existir en múltiples Fincas.
2. Una Finca puede contener múltiples materiales.
3. No deberán existir registros duplicados de `MaterialId + FincaId`.
4. El stock físico solamente deberá modificarse mediante operaciones de inventario.
5. La reserva no deberá modificar el stock físico.
6. La reserva deberá disminuir el stock disponible.
7. El stock disponible deberá calcularse como stock físico menos stock reservado.
8. Una reserva deberá estar vinculada a una Receta, que a su vez, esta vinculada a una Orden de Trabajo.
9. Una reserva deberá estar vinculada a un Material.
10. El consumo deberá afectar el stock físico.
11. El consumo deberá disminuir la reserva correspondiente.
12. Las reservas liberadas deberán volver a estar disponibles.
13. La cancelación de una Orden deberá liberar sus reservas pendientes.
14. Las transferencias deberán afectar correctamente origen y destino.
15. No deberá permitirse superar el stock disponible al generar una reserva.
16. Las operaciones concurrentes deberán proteger la integridad del stock.
17. Los movimientos históricos no deberán eliminarse físicamente sin una estrategia explícita.
18. Los datos históricos de costos no deberán modificarse por cambios posteriores.
19. La creación de una Orden de Trabajo deberá congelar los costos correspondientes.
20. El rendimiento y productividad no forman parte del módulo de Inventario.
21. Al crear una orden de trabajo el stock disponible que hay que tener en cuenta tiene que ver con el stock fisico que hay en la finca a la que se destina dicha orden, no el stock total del material, en caso de que se cree una receta.
22. Cuando una orden de trabajo es cerrada, y en la reserva de los materiales asociados a la receta de esa orden tiene un cantidad reservada de sobra, esta debe liberarse.


---

# 28. Stock negativo

Inicialmente se establece que:

```text
Stock físico < 0 → No permitido
```

El sistema no deberá permitir que una operación genere stock físico negativo, salvo que en el futuro exista una decisión explícita de negocio que modifique esta regla.

Una reserva tampoco podrá superar el stock disponible.

---

# 29. Transferencias entre Fincas

El sistema deberá quedar preparado para permitir posteriormente transferencias.

Ejemplo:

```text
Finca A
Fertilizante X
Stock físico: 500 kg

Transferencia: 100 kg

Finca A:
400 kg

Finca B:
100 kg
```

La transferencia deberá generar conceptualmente:

```text
Movimiento salida → Finca A → 100 kg
Movimiento ingreso → Finca B → 100 kg
```

Las reservas existentes deberán contemplarse para evitar transferir material que se encuentre comprometido.

---

# 30. Integración futura con Compras

El diseño deberá contemplar:

```text
Proveedor
    │
    ▼
Compra
    │
    ▼
DetalleCompra
    │
    ▼
MovimientoInventario
    │
    ▼
StockPorFinca
```

Una compra recibida deberá poder generar automáticamente el ingreso correspondiente al inventario.

El costo unitario de la compra deberá quedar registrado históricamente.

---

# 31. Trazabilidad

El sistema deberá permitir relacionar:

```text
Compra
   ↓
Ingreso
   ↓
Stock por Finca
   ↓
Reserva
   ↓
Orden de Trabajo
   ↓
Consumo
   ↓
Movimiento
```

Esto permitirá posteriormente responder:

* ¿Cuándo ingresó el material?
* ¿De qué compra provino?
* ¿Cuál fue su costo?
* ¿En qué Finca se encuentra?
* ¿Cuánto está reservado?
* ¿Qué Orden de Trabajo lo reservó?
* ¿Cuánto se consumió?
* ¿Cuál era el costo de la Orden?
* ¿Cuál es el stock actual?

---

# 32. Entidades conceptuales principales

## Material

```text
Id
Codigo
Nombre
Descripcion
UnidadMedidaId
CategoriaId
CostoReferencia
Estado
```

## Finca

```text
Id
Nombre
Descripcion
Estado
```

## StockPorFinca

```text
Id
MaterialId
FincaId
StockFisico
StockReservado
StockMinimo
```

Restricción única:

```text
MaterialId + FincaId
```

`StockDisponible` será preferentemente un valor calculado:

```text
StockDisponible = StockFisico - StockReservado
```

## ReservaMaterial

```text
Id
OrdenTrabajoId
MaterialId
FincaId
CantidadReservada
CantidadConsumida
FechaCreacion
FechaLiberacion
Estado
```

La estructura definitiva deberá determinarse durante el diseño técnico.

## MovimientoInventario

```text
Id
MaterialId
FincaId
TipoMovimiento
Cantidad
CostoUnitario
CostoTotal
StockAnterior
StockResultante
Fecha
UsuarioId
Observaciones
Origen
```

## Compra — futura

```text
Id
ProveedorId
Fecha
NumeroComprobante
Estado
Total
```

## DetalleCompra — futura

```text
Id
CompraId
MaterialId
FincaId
Cantidad
CostoUnitario
CostoTotal
```

---

# 33. Transacciones

Las siguientes operaciones deberán considerarse operaciones transaccionales:

### Creación de una Orden con receta

```text
Validar stock disponible
        ↓
Capturar costos históricos
        ↓
Crear Orden
        ↓
Crear reservas
        ↓
Confirmar transacción
```

### Registro de consumo

```text
Validar reserva
        ↓
Validar stock físico
        ↓
Registrar consumo
        ↓
Actualizar reserva
        ↓
Actualizar stock físico
        ↓
Registrar movimiento
        ↓
Confirmar transacción
```

### Cancelación o Cierre de Orden

```text
Identificar reservas pendientes
        ↓
Liberar reservas
        ↓
Actualizar disponibilidad
        ↓
Cancelar o Cerrar Orden
        ↓
Confirmar transacción
```

La implementación deberá evitar estados parciales.

---

# 34. Concurrencia

El sistema deberá contemplar concurrencia al momento de reservar materiales.

No deberá ser posible que dos usuarios creen reservas que individualmente parecen válidas pero que, sumadas, superen el stock disponible.

La implementación deberá utilizar los mecanismos apropiados de SQL Server y Entity Framework Core para garantizar la consistencia.

El agente de desarrollo deberá analizar específicamente:

* Nivel de aislamiento de transacciones.
* Concurrencia optimista/pesimista según corresponda.
* `RowVersion` u otro mecanismo de control de concurrencia.
* Revalidación del stock dentro de la transacción.
* Índices y restricciones de base de datos.
* El uso de SignalR como tecnologia para notificaciones en tiempo real (Ej: stock de un producto bajo). 

No deberá confiarse exclusivamente en una validación previa realizada desde la interfaz.

---

# 35. Informes futuros

El módulo deberá quedar preparado para generar:

### Stock

* Stock físico por Finca.
* Stock reservado por Finca.
* Stock disponible por Finca.
* Stock consolidado.
* Materiales sin stock.
* Materiales por debajo del mínimo.
* Materiales comprometidos por Órdenes de Trabajo.

### Movimientos

* Ingresos.
* Consumos.
* Ajustes.
* Transferencias.
* Historial por material.
* Historial por Finca.

### Costos

* Último costo registrado.
* Historial de costos.
* Costos de adquisición.
* Evolución del costo.

### Órdenes de Trabajo

* Costo estimado.
* Costo histórico.
* Costo de materiales.
* Cantidad planificada.
* Cantidad reservada.
* Cantidad consumida.
* Diferencia entre planificación y consumo.

Los informes históricos deberán utilizar el costo congelado de cada Orden de Trabajo.

---

# 36. Principios de diseño

El desarrollo deberá respetar:

### Trazabilidad

Toda operación relevante deberá poder ser rastreada.

### Historial

Los cambios actuales no deberán destruir información histórica.

### Separación de responsabilidades

Material, Stock, Reservas, Movimientos, Compras y Órdenes de Trabajo deberán mantener responsabilidades diferenciadas.

### Integridad

Las modificaciones del inventario deberán ser consistentes y transaccionales.

### Extensibilidad

La estructura deberá permitir incorporar posteriormente:

* Compras.
* Proveedores.
* Transferencias.
* Valuación.
* Reportes.
* Estadísticas.
* Integraciones externas.
* Productividad de Órdenes de Trabajo.

### Independencia del dominio

El cálculo de rendimiento y productividad no deberá introducir dependencias innecesarias dentro del módulo de Inventario.

---

# 37. Decisiones tomadas

| Tema                    | Decisión                                               |
| ----------------------- | ------------------------------------------------------ |
| Ubicación del stock     | Se controla por Finca / Unidad Productiva              |
| Relación Material-Finca | N:N mediante StockPorFinca                             |
| Stock físico            | Representa existencia física real                      |
| Stock reservado         | Representa cantidades comprometidas pero no consumidas |
| Stock disponible        | Stock físico - Stock reservado                         |
| Reserva                 | Se utilizará reserva blanda                            |
| Momento de reserva      | Al crear una OT con receta                             |
| Reserva física          | No modifica stock físico                               |
| Consumo                 | Descuenta stock físico                                 |
| Consumo                 | Reduce la reserva correspondiente                      |
| Cancelación de OT       | Libera reservas pendientes                             |
| Cierre de OT            | Libera reservas pendientes si las hay                  |
| Stock negativo          | No permitido inicialmente                              |
| Movimientos             | Se registrarán históricamente                          |
| Compras                 | Se contemplarán desde el diseño inicial                |
| Costos                  | Se conservará información histórica                    |
| Costo de OT             | Se congela al crear la OT                              |
| Receta                  | Representa cantidades planificadas                     |
| Reserva                 | Representa compromiso de stock                         |
| Consumo                 | Representa utilización real                            |
| Transferencias          | Se modelarán como movimientos                          |
| Rendimiento             | Fuera del módulo de Inventario                         |
| Productividad           | Se implementará posteriormente                         |
| Base de datos           | Se modificará mediante migraciones controladas         |
| Eventos en tiempo real  | Uso de SignalR para notificaciones en tiempo real      |
| Concurrencia            | Deberá protegerse a nivel transaccional                |
| Valuación               | Método definitivo pendiente de definición              |

---

# 38. Requisitos funcionales específicos

El agente de desarrollo deberá implementar el comportamiento respetando, como mínimo, los siguientes requisitos.

### RF-INV-001 — Stock por Finca

El sistema deberá permitir mantener existencias independientes de un mismo material para diferentes Fincas.

### RF-INV-002 — Stock físico

El sistema deberá mantener la cantidad físicamente existente de cada material por Finca.

### RF-INV-003 — Reserva

El sistema deberá permitir reservar cantidades de materiales para una Orden de Trabajo.

### RF-INV-004 — Reserva automática por receta

Al crear una Orden de Trabajo asociada a una receta, el sistema deberá generar automáticamente las reservas correspondientes a las cantidades planificadas.

### RF-INV-005 — No descontar stock físico

La generación de una reserva no deberá modificar el stock físico.

### RF-INV-006 — Stock disponible

El sistema deberá considerar como disponible únicamente:

```text
Stock físico - Stock reservado
```

### RF-INV-007 — Validación de reserva

El sistema no deberá permitir una reserva superior al stock disponible.

### RF-INV-008 — Consumo

El sistema deberá permitir registrar el consumo real de materiales asociado a una Orden de Trabajo.

### RF-INV-009 — Actualización de stock

Un consumo confirmado deberá disminuir el stock físico correspondiente.

### RF-INV-010 — Actualización de reserva

Un consumo confirmado deberá disminuir la cantidad reservada pendiente de la Orden.

### RF-INV-011 — Liberación de reserva

Las cantidades reservadas que no sean consumidas deberán poder liberarse.

### RF-INV-012 — Cancelación

La cancelación de una Orden deberá liberar las reservas pendientes.

### RF-INV-013 — Costos históricos

El sistema deberá conservar el costo utilizado por una Orden de Trabajo al momento de su creación.

### RF-INV-014 — Independencia del costo actual

Los cambios posteriores del costo de un material no deberán modificar el costo histórico de una Orden existente.

### RF-INV-015 — Movimientos

Toda modificación física del stock deberá generar un movimiento de inventario.

### RF-INV-016 — Trazabilidad

Los movimientos deberán poder relacionarse con su origen.

### RF-INV-017 — Concurrencia

Las operaciones de reserva deberán protegerse contra condiciones de carrera.

### RF-INV-018 — Rendimiento

El módulo de Inventario no deberá implementar cálculos de rendimiento o productividad de las Órdenes de Trabajo.

---

# 39. Criterios de aceptación

El diseño podrá considerarse correcto cuando se cumpla como mínimo:

1. Un material pueda existir en múltiples Fincas.
2. Una Finca pueda contener múltiples materiales.
3. El sistema diferencie stock físico, reservado y disponible.
4. Una reserva no reduzca el stock físico.
5. Una reserva reduzca el stock disponible.
6. No pueda reservarse más material que el disponible.
7. Una Orden con receta genere automáticamente sus reservas.
8. Las reservas estén vinculadas a su Orden de Trabajo.
9. Un consumo reduzca el stock físico.
10. Un consumo reduzca la reserva correspondiente.
11. Una cancelación libere las reservas pendientes.
12. Una modificación de cantidades de la OT actualice correctamente las reservas.
13. Los movimientos físicos queden registrados históricamente.
14. Los costos históricos de las Órdenes permanezcan inalterables.
15. Las operaciones sean transaccionales.
16. Las condiciones de concurrencia no permitan sobre-reservar stock.
17. El stock físico no pueda resultar negativo.
18. El módulo pueda evolucionar posteriormente hacia Compras.
19. El módulo no dependa del futuro sistema de productividad.
20. El modelo permita posteriormente calcular rendimiento utilizando datos de Órdenes de Trabajo y actividades de operarios.

---

# 40. Decisiones pendientes

Antes de comenzar la implementación definitiva deberán resolverse:

1. Si `CostoReferencia` permanecerá directamente en `Material`.
2. Estructura definitiva del historial de costos.
3. Método de valuación del inventario.
4. Estructura definitiva de transferencias.
5. Estructura definitiva de ajustes.
6. Relación exacta entre `MovimientoInventario` y `Compra`.
7. Relación exacta entre `MovimientoInventario` y `Consumo`.
8. Si una compra puede distribuirse entre múltiples Fincas.
9. Estructura definitiva de `ReservaMaterial`.
10. Política para consumos superiores a la cantidad reservada.
11. Comportamiento de las reservas ante modificación de una Orden de Trabajo.
12. Comportamiento de las reservas ante cierre de una Orden.
13. Estrategia concreta de concurrencia en SQL Server / EF Core.
14. Cómo se almacenará técnicamente la instantánea de costo de una Orden.

---

# 41. Funcionalidades explícitamente fuera del alcance

Para evitar que el agente de desarrollo incorpore responsabilidades que no corresponden al módulo, quedan fuera del alcance inicial:

* Cálculo de productividad de operarios.
* Cálculo de rendimiento de tareas.
* Evaluación del rendimiento por hora.
* Indicadores de productividad laboral.
* Gestión de horas trabajadas como módulo de productividad.
* Evaluación de eficiencia operativa.

Estas funcionalidades podrán consumir posteriormente información generada por Órdenes de Trabajo, Actividades y Consumos, pero no deberán formar parte de la implementación inicial de Inventario.

---

# 42. Migraciones

Antes de generar migraciones de Entity Framework Core se deberá revisar el modelo actual de la aplicación.

Entidades prioritarias:

```text
Material
Receta
DetalleReceta
OrdenTrabajo
Consumo
Finca
UnidadMedida
```

Y deberá determinarse si actualmente existen entidades que puedan cumplir las funciones de:

```text
StockPorFinca
ReservaMaterial
MovimientoInventario
HistorialCosto
```

El agente deberá producir antes de modificar la base de datos:

```text
1. Modelo actual.
2. Modelo objetivo.
3. Comparación entre ambos.
4. Entidades nuevas.
5. Entidades modificadas.
6. Relaciones nuevas.
7. Índices.
8. Restricciones.
9. Reglas de negocio.
10. Estrategia de migración de datos.
11. Migraciones EF Core.
12. Riesgos de compatibilidad.
```

No deberán generarse migraciones definitivas hasta completar esta comparación. Y cada cambio debe respetar los principios de arquitectura ya utilizados en el codigo.

---

# 43. Regla arquitectónica fundamental

El diseño deberá respetar la siguiente separación:

```text
                 ORDEN DE TRABAJO
                         │
            ┌────────────┴────────────┐
            │                         │
          Receta                 Actividades
            │                         │
            ▼                         ▼
         Reserva                 Horas trabajadas
            │                         │
            ▼                         │
       INVENTARIO                     │
            │                         │
         Consumo                      │
            │                         │
            ▼                         ▼
     Movimiento                PRODUCTIVIDAD
            │
            ▼
     Stock por Finca
```

**Inventario** será responsable de existencia, reservas, movimientos, consumos y costos relacionados con materiales.

**Órdenes de Trabajo / Productividad** será responsable posteriormente de rendimiento, horas trabajadas y productividad.

Ambos dominios podrán relacionarse, pero no deberán mezclarse sus responsabilidades.

---

**Estado del documento:** Diseño funcional y conceptual.

**Versión:** 0.5.

**Decisiones nuevas incorporadas en esta versión:**

* Reserva blanda de materiales al crear una Orden de Trabajo con receta.
* Separación explícita entre stock físico, reservado y disponible.
* El stock disponible se calcula como `StockFisico - StockReservado`.
* Las reservas no modifican el stock físico.
* Los consumos modifican el stock físico y reducen la reserva.
* Las reservas pendientes se liberan al cancelar o finalizar una Orden.
* El módulo de Inventario no será responsable de rendimiento ni productividad.
* El cálculo de rendimiento queda reservado para una futura evolución del dominio de Órdenes de Trabajo.
* Las operaciones críticas de inventario deberán ser transaccionales y resistentes a problemas de concurrencia.
