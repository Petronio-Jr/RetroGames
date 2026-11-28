import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'

import './index.css'
import './tema-synthwave.css'
import './tema-mobile.css'

import App from './App'
import AutenticacaoProvider from './contexts/AutenticacaoProvider'
import CarrinhoProvider from './contexts/CarrinhoProvider'

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <BrowserRouter>
      <AutenticacaoProvider>
        <CarrinhoProvider>
          <App />
        </CarrinhoProvider>
      </AutenticacaoProvider>
    </BrowserRouter>
  </React.StrictMode>
)
