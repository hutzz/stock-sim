/** @type {import('tailwindcss').Config} */

const defaultTheme = require("tailwindcss/defaultTheme");

module.exports = {
	content: ["./**/*.html", "./**/*.razor"],
	theme: {
		extend: {
			screens: {
				xs: "480px",
				...defaultTheme.screens,
				"3xl": "1920px",
				"4xl": "2560px",
				"5xl": "3440px",
			},
		},
	},
	plugins: [],
};
