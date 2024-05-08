/** @type {import('tailwindcss').Config} */
module.exports = {
	content: ["./**/*.html", "./**/*.razor"],
	theme: {
		extend: {
			screens: {
				"3xl": "1920px",
				"4xl": "2560px",
				"5xl": "3440px",
			},
		},
	},
	plugins: [],
};
