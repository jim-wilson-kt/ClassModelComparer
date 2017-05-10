<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet exclude-result-prefixes="xs" version="1.0"
	xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output doctype-system="about:legacy-compat" encoding="UTF-8" indent="yes" method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>Class Model Comparer</title>
				<meta charset="utf-8"/>
				<meta content="width=device-width, initial-scale=1, shrink-to-fit=no"
					name="viewport"/>
				<link crossorigin="anonymous"
					href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css"
					integrity="sha384-rwoIResjU2yc3z8GV/NPeZWAv56rSmLldC3R/AZzGRnGxQQKnKkoFVhFQhNUwEyJ"
					rel="stylesheet"/>
			</head>
			<body>
				<div class="container" style="margin-top:30px">
					<div class="row">
						<div class="col">
							<ul class="nav nav-pills" role="tablist">
								<li class="nav-item">
									<a class="nav-link active" data-toggle="tab" href="#overview"
										role="tab">Overview</a>
								</li>
								<li class="nav-item">
									<a class="nav-link" data-toggle="tab" href="#argoonly"
										role="tab">Argo-Only</a>
								</li>
								<li class="nav-item">
									<a class="nav-link" data-toggle="tab" href="#dotnetonly"
										role="tab">.NET-Only</a>
								</li>
								<li class="nav-item">
									<a class="nav-link" data-toggle="tab" href="#comparison"
										role="tab">Comparison</a>
								</li>
							</ul>
							<!-- Overview -->
							<div class="tab-content">
								<div class="tab-pane fade in active show" id="overview"
									role="tabpanel">
									<div class="container" style="margin-top:30px">
										<div class="row">
											<div class="col">
												<xsl:apply-templates select="ClassComparison/Config"
												/>
											</div>
										</div>
									</div>
								</div>
								<!-- Agro-Only -->
								<div class="tab-pane fade" id="argoonly" role="tabpanel">
									<div class="container" style="margin-top:30px">
										<div class="row">
											<div class="col">
												<h3>ArgoUML-Only Classes (not in .NET)</h3>
												<ol>
												<xsl:apply-templates mode="namelist"
												select="/ClassComparison/ClassComparison[Existence = 'ArgoOnly']">
												<xsl:sort select="ClassName"/>
												</xsl:apply-templates>
												</ol>
											</div>
										</div>
									</div>
								</div>
								<!-- .NET-Only -->
								<div class="tab-pane fade" id="dotnetonly" role="tabpanel">
									<div class="container" style="margin-top:30px">
										<div class="row">
											<div class="col">
												<h3>.NET-Only Classes (not in ArgoUML)</h3>
												<ol>
												<xsl:apply-templates mode="namelist"
												select="/ClassComparison/ClassComparison[Existence = 'DotNetOnly']">
												<xsl:sort select="ClassName"/>
												</xsl:apply-templates>
												</ol>
											</div>
										</div>
									</div>
								</div>
								<!-- Comparison -->
								<div class="tab-pane fade" id="comparison" role="tabpanel">
									<div class="container" style="margin-top:30px">
										<a name="top"/>
										<div class="row">
											<div class="col">
												<h4>Columns</h4>
												<ol>
												<li>Property Name</li>
												<li>ArgoUML Type / .NET Type</li>
												<li>ArgoUML Min..Max / .NET Min..Max</li>
												</ol>
											</div>
											<div class="col">
												<h4>Badges</h4>
												<li><span class="badge badge-pill badge-danger">A</span><span class="badge badge-pill badge-success">D</span>&#160;ArgoUML class not defined; .NET class defined</li>
												<li><span class="badge badge-pill badge-success">A</span><span class="badge badge-pill badge-success">D</span>&#160;Both ArgoUML and .NET classes defined</li>
												<li><span class="badge badge-pill badge-success">A</span><span class="badge badge-pill badge-danger">D</span>&#160;ArgoUML class defined; .NET class not defined</li>
												<ul>
												<li><span class="badge badge-warning"
												>Argo</span>&#160;ArgoUML Property Missing</li>
												<li><span class="badge badge-warning"
												>.NET</span>&#160;.NET Property Missing</li>
												<li><span class="badge badge-danger"
												>T</span>&#160;Type incompatibility</li>
												<li><span class="badge badge-danger"
												>M</span>&#160;Multiplicity (occurrence)
												mismatch</li>
												</ul>
											</div>
										</div>
										<div class="row">
											<div class="col">
												<div class="dropdown">
												<button aria-expanded="false" aria-haspopup="true"
												class="btn btn-secondary dropdown-toggle"
												data-toggle="dropdown" id="dropdownMenuButton"
												type="button"> Select Class </button>
												<div aria-labelledby="dropdownMenuButton"
												class="dropdown-menu">
												<xsl:apply-templates
												select="ClassComparison/ClassComparison/ClassName">
												<xsl:sort select="."/>
												</xsl:apply-templates>
												</div>
												</div>
												<table
												class="table table-sm table-hover table-bordered table-striped">
												<tbody>
												<xsl:apply-templates mode="comparison"
												select="/ClassComparison/ClassComparison">
												<xsl:sort select="ClassName"/>
												</xsl:apply-templates>
												</tbody>
												</table>
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
				<script crossorigin="anonymous" integrity="sha384-A7FZj7v+d/sdmMqp/nOQwliLvUsJfDHW+k9Omg/a/EheAdgtzNs3hpfag6Ed950n" src="https://code.jquery.com/jquery-3.1.1.slim.min.js"/>
				<script crossorigin="anonymous" integrity="sha384-DztdAPBWPRXSA/3eYEEUWrWCy7G5KFbe8fFjk5JAIxUYHKkDx6Qin1DkWx51bBrb" src="https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js"/>
				<script crossorigin="anonymous" integrity="sha384-vBWWzlZJ8ea9aCX4pEW3rVHjgjt7zpkNpZk+02D9phzyeVkE+jo0ieGizqPLForn" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/js/bootstrap.min.js"/>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="ClassComparison/Config">
		<h3>Source Files</h3>
		<ul>
			<li>
				<xsl:value-of select="ArgoUmlFilepath"/>
			</li>
			<li>
				<xsl:value-of select="DotNetAssemblyFilepath"/>
			</li>
		</ul>
		<h3>Configuration</h3>
		<h4>Ignored Argo Classes</h4>
		<ul>
			<xsl:apply-templates select="ArgoClassIgnore"/>
		</ul>
		<h4>Ignored .NET Classes</h4>
		<ul>
			<xsl:apply-templates select="DotNetClassIgnore"/>
		</ul>
		<h4>Type Equivalencies</h4>
		<ul>
			<table class="table table-sm table-hover table-bordered table-striped">
				<thead class="thead-inverse">
					<tr>
						<th>ArgoUML Type</th>
						<th>.NET Type</th>
					</tr>
				</thead>
				<tbody>
					<xsl:apply-templates select="TypeEquivalency"/>
				</tbody>
			</table>
		</ul>
	</xsl:template>
	<xsl:template match="ArgoClassIgnore">
		<li>
			<xsl:value-of select="."/>
		</li>
	</xsl:template>
	<xsl:template match="DotNetClassIgnore">
		<li>
			<xsl:value-of select="."/>
		</li>
	</xsl:template>
	<xsl:template match="ClassComparison" mode="namelist">
		<li>
			<xsl:value-of select="ClassName"/>
		</li>
	</xsl:template>
	<xsl:template match="TypeEquivalency">
		<tr>
			<td>
				<xsl:value-of select="Argo"/>
			</td>
			<td>
				<xsl:value-of select="DotNet"/>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="ClassComparison/ClassName">
		<a class="dropdown-item" href="#{.}">
			<xsl:value-of select="."/>
		</a>
	</xsl:template>
	<xsl:template match="ClassComparison" mode="comparison">
		<tr class="table-success">
			<td>
				<br />
				<xsl:if test="Existence='DotNetOnly'"><span class="badge badge-pill badge-danger">A</span><span class="badge badge-pill badge-success">D</span></xsl:if>
				<xsl:if test="Existence='Both'"><span class="badge badge-pill badge-success">A</span><span class="badge badge-pill badge-success">D</span></xsl:if>
				<xsl:if test="Existence='ArgoOnly'"><span class="badge badge-pill badge-success">A</span><span class="badge badge-pill badge-danger">D</span></xsl:if>
			</td>
			<td>
				<br />
				<a name="{ClassName}"/>
				<h4><xsl:value-of select="ClassName"/></h4>
			</td>
			<td>
				<br />
				<a class="btn btn-outline-primary btn-sm" href="#top">Top</a>
			</td>
		</tr>
		<xsl:apply-templates select=".[Existence='DotNetOnly' or Existence='ArgoOnly']//UmlClassProperty"/>
		<xsl:apply-templates mode="#default" select="PropertyComparisonList/PropertyComparison"/>
	</xsl:template>
	<xsl:template match="UmlClassProperty">
		<tr>
			<td>
				<xsl:value-of select="Name"/>
			</td>
			<td>
				<a href="#{DataType}"><xsl:value-of select="DataType"/></a>
			</td>
			<td>
				<xsl:value-of
					select="ArgoProperty/MinOccurs"/>..<xsl:value-of select="ArgoProperty/MaxOccurs"
					/>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="PropertyComparison" mode="#default">
		<tr>
			<td>
				<xsl:apply-templates mode="missing-argo-class" select=".[Existence = 'DotNetOnly']"/>
				<xsl:value-of select="PropertyName"/>
				<xsl:apply-templates mode="missing-dotnet-class" select=".[Existence = 'ArgoOnly']"
				/>
			</td>
			<td> <xsl:apply-templates mode="type" select=".[DataTypesAreCompatible = 'false']"/>
				<a href="#{ArgoProperty/DataType}"><xsl:value-of select="ArgoProperty/DataType"/></a>&#160;/&#160; <a href="#{DotNetProperty/DataType}"><xsl:value-of
				select="DotNetProperty/DataType"/></a></td>
			<td> <xsl:apply-templates mode="multiplicity"
				select=".[MinOccursAreSame = 'false' or MaxOccursAreSame = 'false']"/> <xsl:value-of
				select="ArgoProperty/MinOccurs"/>..<xsl:value-of select="ArgoProperty/MaxOccurs"
				/>&#160;/&#160; <xsl:value-of select="DotNetProperty/MinOccurs"/>..<xsl:value-of
				select="DotNetProperty/MaxOccurs"/> </td>
		</tr>
	</xsl:template>
	<xsl:template match="PropertyComparison" mode="type"> <span class="badge badge-danger"
		>T</span>&#160; </xsl:template>
	<xsl:template match="PropertyComparison" mode="multiplicity"> <span class="badge badge-danger"
		>M</span>&#160; </xsl:template>
	<xsl:template match="PropertyComparison" mode="missing-argo-class"> <span
		class="badge badge-warning">Argo</span>&#160;/ </xsl:template>
	<xsl:template match="PropertyComparison" mode="missing-dotnet-class"> /&#160;<span
		class="badge badge-warning">.NET</span> </xsl:template>
</xsl:stylesheet>
