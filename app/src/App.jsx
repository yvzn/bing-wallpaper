/*
   Copyright 2021 Yvan Razafindramanana

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

import React from "react";
import Skeleton from "react-loading-skeleton";

class Wallpapers extends React.Component {
	constructor() {
		const self = super();
		self.state = { images: new Array(4).fill({ status: "loading" }) };
	}

	componentDidMount() {
		var self = this;
		getWallpapers().then((newImages) => {
			self.setState((previousState) => {
				const images = newImages.map((image) => ({
					...image,
					status: "loaded",
				}));
				return {
					...previousState,
					images,
				};
			});
		});
	}

	render() {
		return (
			<section>
				{this.state.images.map((image, index) => (
					<WallpaperCard image={image} key={index} />
				))}
			</section>
		);
	}
}

function WallpaperCard({ image }) {
	return (
		<article>
			<ImageTitle image={image} />
			<ImagePreview image={image} />
			<ImageCopyright image={image} />
		</article>
	);
}

function ImagePreview({ image }) {
	return (
		<p>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && (
				<a href={image.url}>
					<img src={image.url} alt={image.title} />
				</a>
			)}
		</p>
	);
}

function ImageTitle({ image }) {
	return (
		<h2>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && <a href={image.url}>{image.title}</a>}
		</h2>
	);
}

function ImageCopyright({ image }) {
	return (
		<footer>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && image.copyright}
		</footer>
	);
}

function getWallpapers() {
	// Invoke-WebRequest 'https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1' | Select-Object -ExpandProperty Content | ConvertFrom-Json | Select-Object -ExpandProperty images | Select-Object url, copyright, title | ConvertTo-Json
	const images = [
		{
			url: "https://www.bing.com/th?id=OHR.MontChoisy_FR-FR8228486569_640x360.jpg&rf=LaDigue_640x360.jpg&pid=hp",
			copyright:
				"Mont Choisy Beach, plage de l’île Maurice (© Robert Harding World Imagery/Offset by Shutterstock)",
			title: "La force de Maurice",
		},
		{
			url: "https://www.bing.com/th?id=OHR.NgoDong_FR-FR2655491835_640x360.jpg&rf=LaDigue_640x360.jpg&pid=hp",
			copyright:
				"Barques naviguant entre les rizières sur la rivière Ngo Dong, Province de Ninh Binh, Vietnam (© Jeremy Woodhouse/Getty Images)",
			title: "Rivière entre rizières",
		},
		{
			url: "https://www.bing.com/th?id=OHR.July14_FR-FR2238069479_640x360.jpg&rf=LaDigue_640x360.jpg&pid=hp",
			copyright:
				"La Patrouille de France survolant l’Arc de Triomphe lors des célébrations du 14 juillet 2020 (© REUTERS/Gonzalo Fuentes)",
			title: "Du bleu, du blanc et du rouge",
		},
		{
			url: "https://www.bing.com/th?id=OHR.MooseVelvet_FR-FR1300863957_640x360.jpg&rf=LaDigue_640x360.jpg&pid=hp",
			copyright:
				"Élan traversant un étang au pied du mont Moran, Parc national de Grand Teton, Wyoming, États-Unis (© Jim Stamates/Minden Pictures)",
			title: "Dans l’œil d’un artiste",
		},
		{
			url: "https://www.bing.com/th?id=OHR.LighthouseWave_FR-FR0420098693_640x360.jpg&rf=LaDigue_640x360.jpg&pid=hp",
			copyright:
				"Vagues s’écrasant sur le phare de Felgueiras, Porto, Portugal (© Stephan Zirwes/Offset by Shutterstock)",
			title: "Bien arrivé",
		},
	];

	return Promise.resolve(images);
	// replace previous line to simulate loading from a remote source
	// return new Promise((resolve) => setTimeout(resolve, 500, images));
}

function App() {
	return (
		<>
			<Heading />
			<Wallpapers />
			<Footnotes />
		</>
	);
}

function Heading() {
	return (
		<header>
			<h1>Wallpapers!</h1>
		</header>
	);
}

function Footnotes() {
	return (
		<footer>
			<small>
				Images from <a href="https://bing.com">Bing.com</a>. Use of
				images restricted to wallpaper only (as per Bing terms of use).
			</small>
		</footer>
	);
}

export default App;
