// Shared colours and tooltip styling for every Chart.js chart in the app, so a new
// chart inherits the dark shell treatment instead of inventing its own.
const CHART_AXIS_COLOR = "#38383b";
const CHART_TICK_COLOR = "rgba(255, 255, 255, 0.45)";
const CHART_LABEL_COLOR = "rgba(255, 255, 255, 0.72)";
const CHART_ACCENT = "#4b73e3";
const CHART_TOOLTIP = {
	backgroundColor: "rgb(36, 34, 34)",
	borderColor: "rgba(75, 115, 227, 0.45)",
	borderWidth: 1,
	titleColor: "white",
	bodyColor: "rgba(255, 255, 255, 0.8)",
	padding: 10,
	cornerRadius: 8,
};

// One colour per industry sector, keyed by the exact label Yahoo reports. Kept as an
// explicit map (not a rotating palette) so AAPL's slice is the same blue next month.
const SECTOR_COLORS = {
	Technology: "#4b73e3",
	"Consumer Cyclical": "#d98c4f",
	Healthcare: "#4fb3a3",
	"Financial Services": "#7f6fd0",
	Industrials: "#c9a24b",
	Energy: "#b8574f",
	"Communication Services": "#5aa9d6",
	"Consumer Defensive": "#8fae6a",
	Utilities: "#6f9bb5",
	"Basic Materials": "#a8836a",
	"Real Estate": "#9a7fa8",
	Unclassified: "rgba(255, 255, 255, 0.22)",
};

const UNCLASSIFIED_LABEL = "Unclassified";

window.sectorColor = (label) => SECTOR_COLORS[label] || SECTOR_COLORS[UNCLASSIFIED_LABEL];

// rgba() from a hex string, for translucent fills on a dark background.
window.withAlpha = (hex, alpha) => {
	const int = parseInt(hex.slice(1), 16);
	return `rgba(${(int >> 16) & 255}, ${(int >> 8) & 255}, ${int & 255}, ${alpha})`;
};
