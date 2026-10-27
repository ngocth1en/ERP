import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import fs from 'fs';
import path from 'path';

const certsDir = path.resolve(__dirname, './certs');
const keyPath = path.join(certsDir, 'localhost+2-key.pem');
const certPath = path.join(certsDir, 'localhost+2.pem');

// ✅ Auto-detect: dùng HTTPS nếu cert tồn tại, không cần env var
const httpsConfig = fs.existsSync(keyPath) && fs.existsSync(certPath)
    ? {
        key: fs.readFileSync(keyPath),
        cert: fs.readFileSync(certPath),
    }
    : undefined;
export default defineConfig({
    plugins: [react()],
    server: {
        port: 5173,
        proxy: {
            '/api': {
                target: 'https://localhost:7161',
                changeOrigin: true,
                secure: false
            },
            'public/models': {
                target: 'https://localhost:7161', // Trỏ thẳng về backend .NET/Node của bạn
                changeOrigin: true,
                secure: false
            }
        },
        // ===== THÊM CẤU HÌNH HTTPS ĐỂ TEST CAMERA SCAN =====
        https: httpsConfig,
        host: '0.0.0.0',
    },
    build: {
        outDir: 'dist'
    }
})