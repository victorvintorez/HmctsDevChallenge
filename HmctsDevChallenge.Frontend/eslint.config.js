import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tseslint from 'typescript-eslint'
import react from 'eslint-plugin-react'
import pluginRouter from '@tanstack/eslint-plugin-router'
import pluginQuery from '@tanstack/eslint-plugin-query'
import {defineConfig, globalIgnores} from 'eslint/config'

export default defineConfig([
	globalIgnores(['dist']),
	{
		files: ['**/*.{ts,tsx}'],
		extends: [
			js.configs.recommended,
			...tseslint.configs.recommendedTypeChecked,
			...tseslint.configs.stylisticTypeChecked,
			reactHooks.configs.flat.recommended,
			reactRefresh.configs.vite,
			react.configs.flat.recommended,
			pluginRouter.configs["flat/recommended"],
			pluginQuery.configs["flat/recommended"],
			{
				languageOptions: {
					parserOptions: {
						projectService: true
					}
				}
			}
		],
		languageOptions: {
			ecmaVersion: 2020,
			globals: globals.browser,
		},
		rules: {
			...reactHooks.configs.recommended.rules,
			'react-refresh/only-export-components': [
				'warn',
				{allowConstantExport: true},
			],
			...react.configs.recommended.rules,
			...react.configs['jsx-runtime'].rules,
			'react/prop-types': 'off',
			'react/no-children-prop': 'off',
			'@tanstack/router/create-route-property-order': 'error',
			'@tanstack/query/exhaustive-deps': 'error',
		},
		settings: {
			react: {
				version: 'detect',
			}
		}
	},
])
