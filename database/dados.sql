-- RetroGames
-- Script 002: carga de dados iniciais (seed)
-- Execute depois de database/001_criacao_estrutura.sql.
--
-- Este script cadastra dados de domínio e exemplos reais de jogos retrô.
-- Não cadastra usuários: o administrador será criado pela API na etapa
-- de autenticação, conforme a orientação do projeto.
--
-- Os IDs de plataformas e gêneros são explícitos para manter estáveis
-- as referências usadas em tb_jogo e tb_jogo_genero.
-- Pode ser executado novamente sem duplicar os registros deste seed:
-- ON DUPLICATE KEY UPDATE mantém os dados previstos para cada ID/nome.

USE retrogames;

-- 1. Condições com chaves fixas.
INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo)
VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado')
ON DUPLICATE KEY UPDATE
    ds_condicao_jogo = VALUES(ds_condicao_jogo);

-- 2. Perfis com chaves fixas.
INSERT INTO tb_perfil (co_perfil, ds_perfil)
VALUES
    (1, 'Administrador'),
    (2, 'Cliente')
ON DUPLICATE KEY UPDATE
    ds_perfil = VALUES(ds_perfil);

-- 3. Pelo menos seis plataformas reais.
INSERT INTO tb_plataforma
    (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    (1, 'Atari 2600', 'Atari', 1977),
    (2, 'Nintendo Entertainment System (NES)', 'Nintendo', 1983),
    (3, 'Master System', 'Sega', 1985),
    (4, 'Mega Drive', 'Sega', 1988),
    (5, 'Super Nintendo (SNES)', 'Nintendo', 1990),
    (6, 'PlayStation', 'Sony', 1994)
ON DUPLICATE KEY UPDATE
    no_plataforma = VALUES(no_plataforma),
    no_fabricante = VALUES(no_fabricante),
    nu_ano_lancamento = VALUES(nu_ano_lancamento);

-- 4. Oito gêneros.
INSERT INTO tb_genero (co_seq_genero, no_genero)
VALUES
    (1, 'Ação'),
    (2, 'Aventura'),
    (3, 'Plataforma'),
    (4, 'RPG'),
    (5, 'Corrida'),
    (6, 'Luta'),
    (7, 'Esporte'),
    (8, 'Puzzle')
ON DUPLICATE KEY UPDATE
    no_genero = VALUES(no_genero);

-- 5. Vinte jogos reais de consoles retrô.
-- co_condicao_jogo: 1 = Novo, 2 = Usado, 3 = Recondicionado.
-- st_ativo: TRUE = ativo; FALSE = inativo.
-- dt_cadastro usa UTC_TIMESTAMP() para gravar data/hora em UTC.
INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (1, 'Pac-Man', 'Jogo clássico de labirinto e perseguição.', 1980, 89.90, 5, TRUE, UTC_TIMESTAMP(), 1, 2),
    (2, 'Space Invaders', 'Clássico de tiro com invasores alienígenas.', 1978, 99.90, 3, TRUE, UTC_TIMESTAMP(), 1, 2),
    (3, 'Pitfall!', 'Aventura de plataforma na selva.', 1982, 79.90, 0, TRUE, UTC_TIMESTAMP(), 1, 2),

    (4, 'Super Mario Bros.', 'Aventura de plataforma no Reino dos Cogumelos.', 1985, 149.90, 8, TRUE, UTC_TIMESTAMP(), 2, 2),
    (5, 'The Legend of Zelda', 'Aventura de ação em mundo fantástico.', 1986, 189.90, 4, TRUE, UTC_TIMESTAMP(), 2, 3),
    (6, 'Metroid', 'Ação e exploração em cenário de ficção científica.', 1986, 169.90, 2, TRUE, UTC_TIMESTAMP(), 2, 2),
    (7, 'Mega Man 2', 'Jogo de ação e plataforma.', 1988, 159.90, 6, TRUE, UTC_TIMESTAMP(), 2, 2),

    (8, 'Alex Kidd in Miracle World', 'Aventura de plataforma.', 1986, 119.90, 3, TRUE, UTC_TIMESTAMP(), 3, 2),
    (9, 'Wonder Boy III: The Dragon''s Trap', 'Aventura de ação com elementos de RPG.', 1989, 179.90, 2, TRUE, UTC_TIMESTAMP(), 3, 3),
    (10, 'Phantasy Star', 'RPG com exploração e combates por turnos.', 1987, 199.90, 1, TRUE, UTC_TIMESTAMP(), 3, 2),

    (11, 'Sonic the Hedgehog', 'Jogo de plataforma estrelado por Sonic.', 1991, 139.90, 7, TRUE, UTC_TIMESTAMP(), 4, 2),
    (12, 'Streets of Rage 2', 'Ação e luta em progressão lateral.', 1992, 159.90, 4, TRUE, UTC_TIMESTAMP(), 4, 2),
    (13, 'Golden Axe', 'Ação e fantasia medieval.', 1989, 129.90, 0, TRUE, UTC_TIMESTAMP(), 4, 3),
    (14, 'Shinobi III: Return of the Ninja Master', 'Ação e plataforma com temática ninja.', 1993, 169.90, 2, FALSE, UTC_TIMESTAMP(), 4, 2),

    (15, 'Super Mario World', 'Aventura de plataforma em Dinosaur Land.', 1990, 189.90, 5, TRUE, UTC_TIMESTAMP(), 5, 2),
    (16, 'The Legend of Zelda: A Link to the Past', 'Aventura de ação com exploração e RPG.', 1991, 229.90, 3, TRUE, UTC_TIMESTAMP(), 5, 3),
    (17, 'Street Fighter II', 'Jogo de luta um contra um.', 1992, 179.90, 4, TRUE, UTC_TIMESTAMP(), 5, 2),
    (18, 'F-Zero', 'Corrida futurista de alta velocidade.', 1990, 149.90, 2, TRUE, UTC_TIMESTAMP(), 5, 2),

    (19, 'Crash Bandicoot', 'Aventura de plataforma em 3D.', 1996, 99.90, 6, TRUE, UTC_TIMESTAMP(), 6, 2),
    (20, 'Gran Turismo', 'Simulador de corrida automobilística.', 1997, 119.90, 3, TRUE, UTC_TIMESTAMP(), 6, 2)
ON DUPLICATE KEY UPDATE
    no_jogo = VALUES(no_jogo),
    ds_jogo = VALUES(ds_jogo),
    nu_ano_lancamento = VALUES(nu_ano_lancamento),
    vl_preco = VALUES(vl_preco),
    qt_estoque = VALUES(qt_estoque),
    st_ativo = VALUES(st_ativo),
    dt_cadastro = VALUES(dt_cadastro),
    co_plataforma = VALUES(co_plataforma),
    co_condicao_jogo = VALUES(co_condicao_jogo);

-- 6. Associação de cada jogo a um, dois ou três gêneros.
-- INSERT IGNORE evita duplicar associações se o seed for executado novamente.
INSERT IGNORE INTO tb_jogo_genero (co_jogo, co_genero)
VALUES
    (1, 8), (1, 1),
    (2, 1),
    (3, 3), (3, 2),
    (4, 3), (4, 2),
    (5, 2), (5, 1), (5, 4),
    (6, 1), (6, 2),
    (7, 1), (7, 3),
    (8, 3), (8, 2),
    (9, 2), (9, 1), (9, 4),
    (10, 4),
    (11, 3), (11, 1),
    (12, 1), (12, 6),
    (13, 1), (13, 2), (13, 6),
    (14, 1), (14, 3),
    (15, 3), (15, 2),
    (16, 2), (16, 1), (16, 4),
    (17, 6),
    (18, 5),
    (19, 3), (19, 2),
    (20, 5);

-- 7. Consultas de conferência.
-- A tabela tb_usuario é criada pelo script 001, mas permanece vazia nesta etapa.
-- O administrador será cadastrado posteriormente pela API, na aula de autenticação.
SELECT COUNT(*) AS total_usuarios FROM tb_usuario;

SELECT COUNT(*) AS total_condicoes FROM tb_condicao_jogo;
SELECT COUNT(*) AS total_perfis FROM tb_perfil;
SELECT COUNT(*) AS total_plataformas FROM tb_plataforma;
SELECT COUNT(*) AS total_generos FROM tb_genero;
SELECT COUNT(*) AS total_jogos FROM tb_jogo;
SELECT COUNT(*) AS total_associacoes FROM tb_jogo_genero;

-- Deve retornar 20 jogos e entre 1 e 3 gêneros por jogo.
SELECT
    j.co_seq_jogo,
    j.no_jogo,
    COUNT(jg.co_genero) AS quantidade_generos
FROM tb_jogo AS j
LEFT JOIN tb_jogo_genero AS jg
    ON jg.co_jogo = j.co_seq_jogo
GROUP BY j.co_seq_jogo, j.no_jogo
ORDER BY j.co_seq_jogo;

-- Conferência dos requisitos especiais.
SELECT co_seq_jogo, no_jogo, st_ativo, qt_estoque
FROM tb_jogo
WHERE st_ativo = FALSE OR qt_estoque = 0
ORDER BY co_seq_jogo;

