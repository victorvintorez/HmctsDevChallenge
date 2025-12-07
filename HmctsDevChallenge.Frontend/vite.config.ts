import {defineConfig} from 'vite'
import react from '@vitejs/plugin-react'
import {platform} from "node:os";
import {existsSync, mkdirSync, readFileSync} from "node:fs";
import {spawnSync} from "node:child_process";
import {resolve} from "node:path";
import tanstackRouter from "@tanstack/router-plugin/vite";
import {devtools} from "@tanstack/devtools-vite";

// Use .NET dev cert
const appdata =
	platform() === 'win32'
		? process.env.APPDATA
		: platform() === 'linux'
			? `${process.env.HOME}/.local/share`
			: '';
const key = `${appdata}/LicenceManager/cert.key`;
const cert = `${appdata}/LicenceManager/cert.pem`;
if (!existsSync(key) || !existsSync(cert)) {
	mkdirSync(`${appdata}/LicenceManager/`, {recursive: true});
	if (
		0 !==
		spawnSync(
			'dotnet',
			[
				'dev-certs',
				'https',
				'--export-path',
				cert,
				'--format',
				'Pem',
				'--no-password',
			],
			{stdio: 'inherit'},
		).status
	) {
		throw new Error('Failed to generate HTTPS certificate');
	}
}

export default defineConfig({
	plugins: [
		devtools(),
		tanstackRouter({target: 'react', autoCodeSplitting: true}),
		react(),
	],
	server: {
		port: 8082,
		strictPort: true,
		https: {
			key: readFileSync(key),
			cert: readFileSync(cert),
		},
		proxy: {
			'^/api': {
				target: 'https://127.0.0.1:8081',
				secure: false
			}
		}
	},
	resolve: {
		alias: {
			'@assets': resolve(__dirname, 'src/assets'),
			'@components': resolve(__dirname, 'src/components'),
			'@api': resolve(__dirname, 'src/api'),
			'@': resolve(__dirname, 'src'),
		}
	}
})
