
-- ============================================================
-- RETROGAMES
-- Script 003: Consultas do catálogo
-- Referência: Especificação técnica, seções 4.2 e 4.3
-- Fundamentos: Tema 4
-- ============================================================

USE retrogames;


-- ============================================================
-- 1. CATÁLOGO DE JOGOS ATIVOS
-- Exibe o nome do jogo, a plataforma e a descrição da condição.
-- Os resultados são ordenados alfabeticamente pelo nome.
-- ============================================================

SELECT
    j.co_seq_jogo,
    j.no_jogo,
    p.no_plataforma,
    c.ds_condicao_jogo,
    j.nu_ano_lancamento,
    j.vl_preco,
    j.qt_estoque
FROM tb_jogo AS j
INNER JOIN tb_plataforma AS p
    ON p.co_seq_plataforma = j.co_plataforma
INNER JOIN tb_condicao_jogo AS c
    ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo ASC;


-- ============================================================
-- 2. GÊNEROS DE UM JOGO INFORMADO
-- Informe o código do jogo na variável abaixo.
-- Exemplo: @co_seq_jogo = 1
-- ============================================================

SET @co_seq_jogo = 1;

SELECT
    g.co_seq_genero,
    g.no_genero
FROM tb_jogo_genero AS jg
INNER JOIN tb_genero AS g
    ON g.co_seq_genero = jg.co_genero
WHERE jg.co_jogo = @co_seq_jogo
ORDER BY g.no_genero ASC;


-- ============================================================
-- 3. JOGOS DE UM GÊNERO INFORMADO
-- Informe o código do gênero na variável abaixo.
-- Exemplo: @co_seq_genero = 1
-- ============================================================

SET @co_seq_genero = 1;

SELECT
    j.co_seq_jogo,
    j.no_jogo,
    p.no_plataforma,
    j.vl_preco,
    j.qt_estoque,
    j.st_ativo
FROM tb_jogo_genero AS jg
INNER JOIN tb_jogo AS j
    ON j.co_seq_jogo = jg.co_jogo
INNER JOIN tb_plataforma AS p
    ON p.co_seq_plataforma = j.co_plataforma
WHERE jg.co_genero = @co_seq_genero
ORDER BY j.no_jogo ASC;


-- ============================================================
-- 4. QUANTIDADE DE JOGOS POR PLATAFORMA
-- O LEFT JOIN inclui plataformas que não possuem jogos.
-- COUNT conta apenas os jogos encontrados.
-- ============================================================

SELECT
    p.co_seq_plataforma,
    p.no_plataforma,
    COUNT(j.co_seq_jogo) AS qt_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j
    ON j.co_plataforma = p.co_seq_plataforma
GROUP BY
    p.co_seq_plataforma,
    p.no_plataforma
ORDER BY p.no_plataforma ASC;


-- ============================================================
-- 5. BUSCA POR PARTE DO NOME DO JOGO
-- Retorna no máximo 5 itens por página.
-- ============================================================

SET @termo_busca = 'zelda';

SELECT
    co_seq_jogo,
    no_jogo,
    nu_ano_lancamento,
    vl_preco,
    qt_estoque,
    st_ativo
FROM tb_jogo
WHERE no_jogo COLLATE utf8mb4_unicode_ci
    LIKE CONCAT('%', @termo_busca, '%') COLLATE utf8mb4_unicode_ci
ORDER BY no_jogo ASC
LIMIT 5 OFFSET 0;

