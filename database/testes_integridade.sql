START TRANSACTION;

INSERT INTO tb_jogo (
    no_jogo,
    ds_jogo,
    nu_ano_lancamento,
    vl_preco,
    qt_estoque,
    st_ativo,
    dt_cadastro,
    co_plataforma,
    co_condicao_jogo
)
VALUES (
    'Teste Plataforma Inexistente',
    'Teste de chave estrangeira',
    1990,
    100.00,
    5,
    TRUE,
    UTC_TIMESTAMP(),
    999999,
    1
);

ROLLBACK;

-- ============================================================
-- Error Code: 1452. Cannot add or update a child row: a foreign key constraint fails (`retrogames`.`tb_jogo`, CONSTRAINT `fk_jogo_plataforma` FOREIGN KEY (`co_plataforma`) REFERENCES `tb_plataforma` (`co_seq_plataforma`))	0.000 sec
-- ============================================================

START TRANSACTION;

INSERT INTO tb_plataforma (
    no_plataforma,
    no_fabricante,
    nu_ano_lancamento
)
VALUES (
    'Atari 2600',
    'Atari',
    1977
);

ROLLBACK;

-- ============================================================
-- Error Code: 1062. Duplicate entry 'Atari 2600' for key 'tb_plataforma.uq_plataforma_nome'	0.000 sec
-- ============================================================

START TRANSACTION;

DELETE FROM tb_plataforma
WHERE co_seq_plataforma = 1;

ROLLBACK;

-- ============================================================
-- Error Code: 1451. Cannot delete or update a parent row: a foreign key constraint fails (`retrogames`.`tb_jogo`, CONSTRAINT `fk_jogo_plataforma` FOREIGN KEY (`co_plataforma`) REFERENCES `tb_plataforma` (`co_seq_plataforma`))	0.000 sec
-- ============================================================

START TRANSACTION;

INSERT INTO tb_jogo (
    no_jogo,
    ds_jogo,
    nu_ano_lancamento,
    vl_preco,
    qt_estoque,
    st_ativo,
    dt_cadastro,
    co_plataforma,
    co_condicao_jogo
)
VALUES (
    NULL,
    'Teste de nome nulo',
    1990,
    100.00,
    5,
    TRUE,
    UTC_TIMESTAMP(),
    1,
    1
);

ROLLBACK;

-- ============================================================
-- Error Code: 1048. Column 'no_jogo' cannot be null	0.000 sec
-- ============================================================

INSERT INTO tb_jogo (
    no_jogo,
    ds_jogo,
    nu_ano_lancamento,
    vl_preco,
    qt_estoque,
    st_ativo,
    dt_cadastro,
    co_plataforma,
    co_condicao_jogo
)
VALUES (
    'Teste Preco Zero',
    'Teste de preço igual a zero',
    1990,
    0.00,
    5,
    TRUE,
    UTC_TIMESTAMP(),
    1,
    1
);

DELETE FROM tb_jogo
WHERE no_jogo = 'Teste Preco Zero';

-- ============================================================
-- 1 row(s) affected
-- ============================================================