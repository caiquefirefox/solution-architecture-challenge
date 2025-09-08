import type { Config } from 'tailwindcss'
export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: { brand: { 50:'#eef8ff',100:'#d9ecff',200:'#b9dcff',300:'#8cc6ff',400:'#5eaaff',500:'#3c8dff',600:'#286fe6',700:'#1f57b4',800:'#1d4a91',900:'#1b3f78' } }
    }
  },
  plugins: []
} satisfies Config
