-- RetroGames
-- Script 001: criação da estrutura do banco de dados
-- Referência: Especificação técnica, seções 4.1 a 4.3.
--
-- Para recriar o banco do zero, execute ANTES deste script:
-- DROP DATABASE IF EXISTS retrogames;
-- Atenção: o comando acima apaga permanentemente o banco e seus dados.

CREATE DATABASE IF NOT EXISTS retrogames
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE retrogames;

-- Tabela de domínio: condições de conservação dos jogos.
CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo SMALLINT UNSIGNED NOT NULL,
    ds_condicao_jogo VARCHAR(100) NOT NULL,
    CONSTRAINT pk_condicao_jogo PRIMARY KEY (co_condicao_jogo),
    CONSTRAINT uq_condicao_jogo_descricao UNIQUE (ds_condicao_jogo)
) ENGINE = InnoDB;

-- Tabela de domínio: perfis de acesso.
CREATE TABLE tb_perfil (
    co_perfil SMALLINT UNSIGNED NOT NULL,
    ds_perfil VARCHAR(50) NOT NULL,
    CONSTRAINT pk_perfil PRIMARY KEY (co_perfil),
    CONSTRAINT uq_perfil_descricao UNIQUE (ds_perfil)
) ENGINE = InnoDB;

-- Cadastro de plataformas.
CREATE TABLE tb_plataforma (
    co_seq_plataforma INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(100) NOT NULL,
    no_fabricante VARCHAR(100) NOT NULL,
    nu_ano_lancamento SMALLINT UNSIGNED NOT NULL,
    CONSTRAINT pk_plataforma PRIMARY KEY (co_seq_plataforma),
    CONSTRAINT uq_plataforma_nome UNIQUE (no_plataforma)
) ENGINE = InnoDB;

-- Cadastro de gêneros.
CREATE TABLE tb_genero (
    co_seq_genero INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(80) NOT NULL,
    CONSTRAINT pk_genero PRIMARY KEY (co_seq_genero),
    CONSTRAINT uq_genero_nome UNIQUE (no_genero)
) ENGINE = InnoDB;

-- Cadastro principal de jogos.
CREATE TABLE tb_jogo (
    co_seq_jogo INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_jogo VARCHAR(150) NOT NULL,
    ds_jogo TEXT NULL,
    nu_ano_lancamento SMALLINT UNSIGNED NOT NULL,
    vl_preco DECIMAL(10,2) NOT NULL,
    qt_estoque INT UNSIGNED NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_plataforma INT UNSIGNED NOT NULL,
    co_condicao_jogo SMALLINT UNSIGNED NOT NULL,
    CONSTRAINT pk_jogo PRIMARY KEY (co_seq_jogo),
    INDEX idx_jogo_nome (no_jogo),
    CONSTRAINT fk_jogo_plataforma
        FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma (co_seq_plataforma),
    CONSTRAINT fk_jogo_condicao
        FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo (co_condicao_jogo)
) ENGINE = InnoDB;

-- Associação N:N entre jogos e gêneros.
CREATE TABLE tb_jogo_genero (
    co_jogo INT UNSIGNED NOT NULL,
    co_genero INT UNSIGNED NOT NULL,
    CONSTRAINT pk_jogo_genero PRIMARY KEY (co_jogo, co_genero),
    CONSTRAINT fk_jogo_genero_jogo
        FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo),
    CONSTRAINT fk_jogo_genero_genero
        FOREIGN KEY (co_genero)
        REFERENCES tb_genero (co_seq_genero)
) ENGINE = InnoDB;

-- Usuários do sistema.
CREATE TABLE tb_usuario (
    co_seq_usuario INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_usuario VARCHAR(120) NOT NULL,
    ds_email VARCHAR(254) NOT NULL,
    ds_senha_hash VARCHAR(255) NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_perfil SMALLINT UNSIGNED NOT NULL,
    CONSTRAINT pk_usuario PRIMARY KEY (co_seq_usuario),
    CONSTRAINT uq_usuario_email UNIQUE (ds_email),
    CONSTRAINT fk_usuario_perfil
        FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil (co_perfil)
) ENGINE = InnoDB;

-- Conferência: deve listar as sete tabelas do dicionário de dados.
SHOW TABLES;