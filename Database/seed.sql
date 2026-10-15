-- Datos de prueba de BancoPanda. Solo para desarrollo.
--
-- Crea dos usuarios con sus cuentas, tarjetas y movimientos:
--   andrea.barroso@bancopanda.test  (cédula 0912345678)
--   carlos.mendoza@bancopanda.test  (cédula 0923456781)
-- Contraseña de ambos: Panda2026
--
-- Se puede ejecutar las veces que haga falta: primero borra lo que sembró
-- antes y lo vuelve a crear, así las fechas quedan siempre recientes.
-- No toca a los usuarios registrados desde la app.

SET client_encoding = 'UTF8';

BEGIN;

DELETE FROM usuarios
WHERE usuario_id IN (
    'a1000000-0000-4000-8000-000000000001',
    'a1000000-0000-4000-8000-000000000002'
);

-- contrasena_hash es el hash BCrypt de "Panda2026", el mismo formato que genera el API.
INSERT INTO usuarios (usuario_id, nombres, apellidos, cedula, correo, telefono, contrasena_hash, fecha_registro)
VALUES
    ('a1000000-0000-4000-8000-000000000001', 'Andrea', 'Barroso Torres', '0912345678',
     'andrea.barroso@bancopanda.test', '0987654321',
     '$2a$11$FiLbRvl0XcURIshFBO0Xf.xzT43MeGm9d5RKhQlc8gx6utomu7BbG', now() - interval '45 days'),
    ('a1000000-0000-4000-8000-000000000002', 'Carlos', 'Mendoza Vera', '0923456781',
     'carlos.mendoza@bancopanda.test', '0991234567',
     '$2a$11$FiLbRvl0XcURIshFBO0Xf.xzT43MeGm9d5RKhQlc8gx6utomu7BbG', now() - interval '20 days');

-- El saldo se calcula al final a partir de los movimientos.
INSERT INTO cuentas (cuenta_id, numero_cuenta, tipo, saldo, moneda, fecha_apertura, usuario_id)
VALUES
    ('c1000000-0000-4000-8000-000000000001', '5000900001', 'Ahorros',   0, 'USD', now() - interval '45 days', 'a1000000-0000-4000-8000-000000000001'),
    ('c1000000-0000-4000-8000-000000000002', '5000900002', 'Corriente', 0, 'USD', now() - interval '35 days', 'a1000000-0000-4000-8000-000000000001'),
    ('c1000000-0000-4000-8000-000000000003', '5000900003', 'Ahorros',   0, 'USD', now() - interval '20 days', 'a1000000-0000-4000-8000-000000000002');

INSERT INTO tarjetas (tarjeta_id, ultimos_digitos, marca, tipo, fecha_expiracion, activa, cuenta_id)
VALUES
    ('d1000000-0000-4000-8000-000000000001', '4821', 'Visa',       'Debito',  (current_date + interval '3 years')::date, true,  'c1000000-0000-4000-8000-000000000001'),
    ('d1000000-0000-4000-8000-000000000002', '7730', 'Mastercard', 'Debito',  (current_date + interval '4 years')::date, true,  'c1000000-0000-4000-8000-000000000002'),
    ('d1000000-0000-4000-8000-000000000003', '1596', 'Visa',       'Debito',  (current_date + interval '4 years')::date, true,  'c1000000-0000-4000-8000-000000000003'),
    ('d1000000-0000-4000-8000-000000000004', '3068', 'Mastercard', 'Credito', (current_date + interval '1 year')::date,  false, 'c1000000-0000-4000-8000-000000000003');

INSERT INTO movimientos (movimiento_id, descripcion, monto, tipo, categoria, fecha, cuenta_id)
SELECT gen_random_uuid(), m.descripcion, m.monto, m.tipo, m.categoria, now() - m.hace, m.cuenta_id
FROM (VALUES
    -- Andrea · cuenta de ahorros
    ('McDonalds',                                   9.75, 'Egreso',  'Comida',        interval '2 hours',          'c1000000-0000-4000-8000-000000000001'::uuid),
    ('Deprati',                                    84.20, 'Egreso',  'Compras',       interval '5 hours',          'c1000000-0000-4000-8000-000000000001'),
    ('Supermaxi',                                  63.18, 'Egreso',  'Compras',       interval '1 day 3 hours',    'c1000000-0000-4000-8000-000000000001'),
    ('Uber',                                        4.60, 'Egreso',  'Transporte',    interval '1 day 9 hours',    'c1000000-0000-4000-8000-000000000001'),
    ('Pago de nómina',                           1850.00, 'Ingreso', 'Sueldo',        interval '2 days',           'c1000000-0000-4000-8000-000000000001'),
    ('CNEL - Planilla de luz',                     38.45, 'Egreso',  'Servicios',     interval '3 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Netflix',                                    10.99, 'Egreso',  'Servicios',     interval '4 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Sweet & Coffee',                              6.80, 'Egreso',  'Comida',        interval '5 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Transferencia recibida de Carlos Mendoza',  120.00, 'Ingreso', 'Transferencia', interval '6 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Transferencia a María López',               250.00, 'Egreso',  'Transferencia', interval '8 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Farmacias Fybeca',                           22.35, 'Egreso',  'Otros',         interval '10 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Metrovía - Recarga',                          5.00, 'Egreso',  'Transporte',    interval '12 days',          'c1000000-0000-4000-8000-000000000001'),
    ('KFC',                                        14.25, 'Egreso',  'Comida',        interval '15 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Claro - Plan móvil',                         25.76, 'Egreso',  'Servicios',     interval '18 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Mi Comisariato',                             47.90, 'Egreso',  'Compras',       interval '21 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Interagua - Planilla de agua',               18.32, 'Egreso',  'Servicios',     interval '25 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Pago de nómina',                           1850.00, 'Ingreso', 'Sueldo',        interval '32 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Depósito en ventanilla',                   9238.35, 'Ingreso', 'Otros',         interval '40 days',          'c1000000-0000-4000-8000-000000000001'),

    -- Andrea · cuenta corriente
    ('Pago de arriendo',                          450.00, 'Egreso',  'Servicios',     interval '4 hours',          'c1000000-0000-4000-8000-000000000002'),
    ('Pizza Hut',                                  21.50, 'Egreso',  'Comida',        interval '20 hours',         'c1000000-0000-4000-8000-000000000002'),
    ('Spotify',                                     5.99, 'Egreso',  'Servicios',     interval '2 days',           'c1000000-0000-4000-8000-000000000002'),
    ('Gasolinera Primax',                          30.00, 'Egreso',  'Transporte',    interval '3 days',           'c1000000-0000-4000-8000-000000000002'),
    ('Amazon',                                     58.90, 'Egreso',  'Compras',       interval '7 days',           'c1000000-0000-4000-8000-000000000002'),
    ('Transferencia recibida de Luis Torres',     300.00, 'Ingreso', 'Transferencia', interval '9 days',           'c1000000-0000-4000-8000-000000000002'),
    ('Pago de arriendo',                          450.00, 'Egreso',  'Servicios',     interval '30 days',          'c1000000-0000-4000-8000-000000000002'),
    ('Depósito en ventanilla',                   3000.00, 'Ingreso', 'Otros',         interval '35 days',          'c1000000-0000-4000-8000-000000000002'),

    -- Carlos · cuenta de ahorros
    ('Cinemark',                                   12.00, 'Egreso',  'Otros',         interval '6 hours',          'c1000000-0000-4000-8000-000000000003'),
    ('Menestras del Negro',                         8.50, 'Egreso',  'Comida',        interval '1 day 2 hours',    'c1000000-0000-4000-8000-000000000003'),
    ('CNT - Internet',                             28.00, 'Egreso',  'Servicios',     interval '2 days',           'c1000000-0000-4000-8000-000000000003'),
    ('Cooperativa de taxis',                        3.50, 'Egreso',  'Transporte',    interval '3 days',           'c1000000-0000-4000-8000-000000000003'),
    ('Almacenes Tía',                              32.15, 'Egreso',  'Compras',       interval '5 days',           'c1000000-0000-4000-8000-000000000003'),
    ('Transferencia a Andrea Barroso',            120.00, 'Egreso',  'Transferencia', interval '6 days',           'c1000000-0000-4000-8000-000000000003'),
    ('Pago de nómina',                           1200.00, 'Ingreso', 'Sueldo',        interval '14 days',          'c1000000-0000-4000-8000-000000000003'),
    ('Bono de bienvenida',                        100.00, 'Ingreso', 'Otros',         interval '20 days',          'c1000000-0000-4000-8000-000000000003')
) AS m (descripcion, monto, tipo, categoria, hace, cuenta_id);

UPDATE cuentas c
SET saldo = (
    SELECT COALESCE(SUM(CASE m.tipo WHEN 'Ingreso' THEN m.monto ELSE -m.monto END), 0)
    FROM movimientos m
    WHERE m.cuenta_id = c.cuenta_id
)
WHERE c.usuario_id IN (
    'a1000000-0000-4000-8000-000000000001',
    'a1000000-0000-4000-8000-000000000002'
);

COMMIT;
