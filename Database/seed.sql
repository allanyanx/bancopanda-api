-- Datos de prueba de BancoPanda. Solo para desarrollo.
--
-- Crea dos usuarios con sus cuentas, tarjetas y movimientos:
--   andrea.demo  (andrea.barroso@bancopanda.test, cédula 0912345678)
--   carlos.demo  (carlos.mendoza@bancopanda.test, cédula 0923456781)
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
INSERT INTO usuarios (usuario_id, nombre_usuario, nombres, apellidos, cedula, correo, telefono, contrasena_hash, fecha_registro)
VALUES
    ('a1000000-0000-4000-8000-000000000001', 'andrea.demo', 'Andrea', 'Barroso Torres', '0912345678',
     'andrea.barroso@bancopanda.test', '+593987654321',
     '$2a$11$FiLbRvl0XcURIshFBO0Xf.xzT43MeGm9d5RKhQlc8gx6utomu7BbG', now() - interval '45 days'),
    ('a1000000-0000-4000-8000-000000000002', 'carlos.demo', 'Carlos', 'Mendoza Vera', '0923456781',
     'carlos.mendoza@bancopanda.test', '+593991234567',
     '$2a$11$FiLbRvl0XcURIshFBO0Xf.xzT43MeGm9d5RKhQlc8gx6utomu7BbG', now() - interval '20 days');

-- El saldo se calcula al final a partir de los movimientos.
INSERT INTO cuentas (cuenta_id, numero_cuenta, tipo, saldo, moneda, fecha_apertura, usuario_id)
VALUES
    ('c1000000-0000-4000-8000-000000000001', '5000900001', 'Ahorros',   0, 'USD', now() - interval '45 days', 'a1000000-0000-4000-8000-000000000001'),
    ('c1000000-0000-4000-8000-000000000002', '5000900002', 'Corriente', 0, 'USD', now() - interval '35 days', 'a1000000-0000-4000-8000-000000000001'),
    ('c1000000-0000-4000-8000-000000000003', '5000900003', 'Ahorros',   0, 'USD', now() - interval '20 days', 'a1000000-0000-4000-8000-000000000002');

INSERT INTO tarjetas (tarjeta_id, numero_tarjeta, marca, tipo, fecha_expiracion, activa, cuenta_id)
VALUES
    ('d1000000-0000-4000-8000-000000000001', '4754009171454821', 'Visa',       'Debito',  (current_date + interval '3 years')::date, true,  'c1000000-0000-4000-8000-000000000001'),
    ('d1000000-0000-4000-8000-000000000002', '5317160790967730', 'Mastercard', 'Debito',  (current_date + interval '4 years')::date, true,  'c1000000-0000-4000-8000-000000000002'),
    ('d1000000-0000-4000-8000-000000000003', '4535572117931596', 'Visa',       'Debito',  (current_date + interval '4 years')::date, true,  'c1000000-0000-4000-8000-000000000003'),
    ('d1000000-0000-4000-8000-000000000004', '5129922174033068', 'Mastercard', 'Credito', (current_date + interval '1 year')::date,  false, 'c1000000-0000-4000-8000-000000000003');

-- Beneficiarios frecuentes de Andrea. Los bancos los crea la migración.
INSERT INTO beneficiarios (beneficiario_id, numero_cuenta, cedula, nombre, usuario_id, banco_id)
VALUES
    ('e1000000-0000-4000-8000-000000000001', '5000900003', '0923456781', 'Carlos Mendoza', 'a1000000-0000-4000-8000-000000000001', 'b0000000-0000-4000-8000-000000000001'),
    ('e1000000-0000-4000-8000-000000000002', '2200451873', '0915678234', 'María López',    'a1000000-0000-4000-8000-000000000001', 'b0000000-0000-4000-8000-000000000003'),
    ('e1000000-0000-4000-8000-000000000003', '0034561290', '0918273645', 'Luis Torres',    'a1000000-0000-4000-8000-000000000001', 'b0000000-0000-4000-8000-000000000002');

INSERT INTO movimientos (movimiento_id, descripcion, monto, tipo, categoria, fecha, cuenta_id)
SELECT gen_random_uuid(), m.descripcion, m.monto, m.tipo, m.categoria, now() - m.hace, m.cuenta_id
FROM (VALUES
    -- Andrea · cuenta de ahorros
    ('McDonalds',                                   4.50, 'Egreso',  'Comida',        interval '2 hours',          'c1000000-0000-4000-8000-000000000001'::uuid),
    ('Deprati',                                    12.00, 'Egreso',  'Compras',       interval '5 hours',          'c1000000-0000-4000-8000-000000000001'),
    ('Supermaxi',                                  18.25, 'Egreso',  'Compras',       interval '1 day 3 hours',    'c1000000-0000-4000-8000-000000000001'),
    ('Uber',                                        3.10, 'Egreso',  'Transporte',    interval '1 day 9 hours',    'c1000000-0000-4000-8000-000000000001'),
    ('Pago de nómina',                             80.00, 'Ingreso', 'Sueldo',        interval '2 days',           'c1000000-0000-4000-8000-000000000001'),
    ('CNEL - Planilla de luz',                      9.40, 'Egreso',  'Servicios',     interval '3 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Netflix',                                     5.99, 'Egreso',  'Servicios',     interval '4 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Sweet & Coffee',                              3.75, 'Egreso',  'Comida',        interval '5 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Transferencia recibida de Carlos Mendoza',   15.00, 'Ingreso', 'Transferencia', interval '6 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Transferencia a María López',                20.00, 'Egreso',  'Transferencia', interval '8 days',           'c1000000-0000-4000-8000-000000000001'),
    ('Farmacias Fybeca',                            6.35, 'Egreso',  'Otros',         interval '10 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Metrovía - Recarga',                          2.00, 'Egreso',  'Transporte',    interval '12 days',          'c1000000-0000-4000-8000-000000000001'),
    ('KFC',                                         7.50, 'Egreso',  'Comida',        interval '15 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Claro - Plan móvil',                         10.00, 'Egreso',  'Servicios',     interval '18 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Mi Comisariato',                             15.80, 'Egreso',  'Compras',       interval '21 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Interagua - Planilla de agua',                6.20, 'Egreso',  'Servicios',     interval '25 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Pago de nómina',                             80.00, 'Ingreso', 'Sueldo',        interval '32 days',          'c1000000-0000-4000-8000-000000000001'),
    ('Depósito en ventanilla',                     49.84, 'Ingreso', 'Otros',         interval '40 days',          'c1000000-0000-4000-8000-000000000001'),

    -- Andrea · cuenta corriente
    ('Pago de arriendo',                           35.00, 'Egreso',  'Servicios',     interval '4 hours',          'c1000000-0000-4000-8000-000000000002'),
    ('Pizza Hut',                                   8.50, 'Egreso',  'Comida',        interval '20 hours',         'c1000000-0000-4000-8000-000000000002'),
    ('Spotify',                                     5.99, 'Egreso',  'Servicios',     interval '2 days',           'c1000000-0000-4000-8000-000000000002'),
    ('Gasolinera Primax',                          10.00, 'Egreso',  'Transporte',    interval '3 days',           'c1000000-0000-4000-8000-000000000002'),
    ('Amazon',                                     12.51, 'Egreso',  'Compras',       interval '7 days',           'c1000000-0000-4000-8000-000000000002'),
    ('Transferencia recibida de Luis Torres',      30.00, 'Ingreso', 'Transferencia', interval '9 days',           'c1000000-0000-4000-8000-000000000002'),
    ('Pago de arriendo',                           35.00, 'Egreso',  'Servicios',     interval '30 days',          'c1000000-0000-4000-8000-000000000002'),
    ('Depósito en ventanilla',                    177.00, 'Ingreso', 'Otros',         interval '35 days',          'c1000000-0000-4000-8000-000000000002'),

    -- Carlos · cuenta de ahorros
    ('Cinemark',                                    6.00, 'Egreso',  'Otros',         interval '6 hours',          'c1000000-0000-4000-8000-000000000003'),
    ('Menestras del Negro',                         4.50, 'Egreso',  'Comida',        interval '1 day 2 hours',    'c1000000-0000-4000-8000-000000000003'),
    ('CNT - Internet',                             15.00, 'Egreso',  'Servicios',     interval '2 days',           'c1000000-0000-4000-8000-000000000003'),
    ('Cooperativa de taxis',                        2.25, 'Egreso',  'Transporte',    interval '3 days',           'c1000000-0000-4000-8000-000000000003'),
    ('Almacenes Tía',                               9.75, 'Egreso',  'Compras',       interval '5 days',           'c1000000-0000-4000-8000-000000000003'),
    ('Transferencia a Andrea Barroso',             15.00, 'Egreso',  'Transferencia', interval '6 days',           'c1000000-0000-4000-8000-000000000003'),
    ('Pago de nómina',                             52.50, 'Ingreso', 'Sueldo',        interval '14 days',          'c1000000-0000-4000-8000-000000000003'),
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
